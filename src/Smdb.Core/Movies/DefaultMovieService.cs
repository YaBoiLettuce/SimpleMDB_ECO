namespace Smdb.Core.Movies;

using Shared.Http;
using System.Net;

// Implementa la lógica de negocio para películas, usando un repositorio
// Retorna Result<T> para encapsular tanto payload como errores HTTP
public class DefaultMovieService : IMovieService
{
    private IMovieRepository movieRepository;

    public DefaultMovieService(IMovieRepository movieRepository)
    {
        this.movieRepository = movieRepository;
    }

    // Lee un conjunto paginado de películas
    public async Task<Result<PagedResult<Movie>>> ReadMovies(int page, int size)
    {
        if (page < 1)
        {
            // Validación: la página no puede ser menor a 1
            return new Result<PagedResult<Movie>>(
              new Exception("Page must be >= 1."),
              (int)HttpStatusCode.BadRequest);
        }

        if (size < 1)
        {
            // Validación: el tamaño de página no puede ser menor a 1
            return new Result<PagedResult<Movie>>(
              new Exception("Page size must be >= 1."),
              (int)HttpStatusCode.BadRequest);
        }

        var pagedResult = await movieRepository.ReadMovies(page, size);
        if (pagedResult == null)
        {
            // Validación: si no se pudieron leer películas del repositorio
            return new Result<PagedResult<Movie>>(
                new Exception($"Could not read movies from page {page} and size {size}."),
                (int)HttpStatusCode.NotFound);
        }

        return new Result<PagedResult<Movie>>(pagedResult, (int)HttpStatusCode.OK);
    }

    // Crea una película nueva
    public async Task<Result<Movie>> CreateMovie(Movie newMovie)
    {
        var validationResult = ValidateMovie(newMovie);
        if (validationResult != null)
        {
            // Validación: datos de película no válidos
            return validationResult;
        }

        var movie = await movieRepository.CreateMovie(newMovie);
        if (movie == null)
        {
            // Validación: si el repositorio no pudo crear la película
            return new Result<Movie>(
                new Exception($"Could not create movie {newMovie}."),
                (int)HttpStatusCode.NotFound);
        }

        return new Result<Movie>(movie, (int)HttpStatusCode.Created);
    }

    // Lee una película específica por ID
    public async Task<Result<Movie>> ReadMovie(int id)
    {
        var movie = await movieRepository.ReadMovie(id);
        if (movie == null)
        {
            // Validación: película no encontrada
            return new Result<Movie>(
                new Exception($"Could not read movie with id {id}."),
                (int)HttpStatusCode.NotFound);
        }

        return new Result<Movie>(movie, (int)HttpStatusCode.OK);
    }

    // Actualiza una película existente
    public async Task<Result<Movie>> UpdateMovie(int id, Movie newData)
    {
        var validationResult = ValidateMovie(newData);
        if (validationResult != null)
        {
            // Validación: datos de película no válidos
            return validationResult;
        }

        var movie = await movieRepository.UpdateMovie(id, newData);
        if (movie == null)
        {
            // Validación: no se encontró la película para actualizar
            return new Result<Movie>(
                new Exception($"Could not update movie {newData} with id {id}."),
                (int)HttpStatusCode.NotFound);
        }

        return new Result<Movie>(movie, (int)HttpStatusCode.OK);
    }

    // Elimina una película por ID
    public async Task<Result<Movie>> DeleteMovie(int id)
    {
        var movie = await movieRepository.DeleteMovie(id);
        if (movie == null)
        {
            // Validación: no se encontró la película para eliminar
            return new Result<Movie>(
                new Exception($"Could not delete movie with id {id}."),
                (int)HttpStatusCode.NotFound);
        }

        return new Result<Movie>(movie, (int)HttpStatusCode.OK);
    }

    // Función privada que valida los datos de la película
    private static Result<Movie>? ValidateMovie(Movie? movieData)
    {
        if (movieData is null)
        {
            // Validación: el payload de la película es requerido
            return new Result<Movie>(
                new Exception("Movie payload is required."),
                (int)HttpStatusCode.BadRequest);
        }

        if (string.IsNullOrWhiteSpace(movieData.Title))
        {
            // Validación: el título es obligatorio
            return new Result<Movie>(
                new Exception("Title is required and cannot be empty."),
                (int)HttpStatusCode.BadRequest);
        }

        if (movieData.Title.Length > 256)
        {
            // Validación: longitud máxima del título
            return new Result<Movie>(
                new Exception("Title cannot be longer than 256 characters."),
                (int)HttpStatusCode.BadRequest);
        }

        if (movieData.Year < 1888 || movieData.Year > DateTime.UtcNow.Year)
        {
            // Validación: año de película dentro de rango válido
            return new Result<Movie>(
                new Exception($"Year must be between 1888 and {DateTime.UtcNow.Year}."),
                (int)HttpStatusCode.BadRequest);
        }

        return null; // Datos válidos
    }
}