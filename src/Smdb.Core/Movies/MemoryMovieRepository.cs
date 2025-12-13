namespace Smdb.Core.Movies;

using Shared.Http;
using Smdb.Core.Db;

public class MemoryMovieRepository : IMovieRepository
{
    private MemoryDatabase db;

    // Constructor: inyecta la base de datos en memoria para operar sobre ella
    public MemoryMovieRepository(MemoryDatabase db)
    {
        this.db = db;
    }

    // Devuelve una lista paginada de películas
    public async Task<PagedResult<Movie>?> ReadMovies(int page, int size)
    {
        int totalCount = db.Movies.Count; // Total de películas en la DB
        int start = Math.Clamp((page - 1) * size, 0, totalCount); // Índice inicial
        int length = Math.Clamp(size, 0, totalCount - start); // Cantidad de elementos a devolver
        var values = db.Movies.Slice(start, length); // Sublista de películas
        var result = new PagedResult<Movie>(totalCount, values); // Empaqueta resultado paginado

        return await Task.FromResult(result); // Retorna el resultado
    }

    // Crea una nueva película en la base de datos
    public async Task<Movie?> CreateMovie(Movie newMovie)
    {
        newMovie.Id = db.NextMovieId(); // Asigna un ID único
        db.Movies.Add(newMovie); // Agrega la película a la DB

        return await Task.FromResult(newMovie); // Retorna la película creada
    }

    // Lee una película específica por su ID
    public async Task<Movie?> ReadMovie(int id)
    {
        Movie? result = db.Movies.FirstOrDefault(m => m.Id == id); // Busca la película
        return await Task.FromResult(result); // Retorna el resultado (o null si no existe)
    }

    // Actualiza los datos de una película existente
    public async Task<Movie?> UpdateMovie(int id, Movie newData)
    {
        Movie? result = db.Movies.FirstOrDefault(m => m.Id == id); // Busca la película

        if (result != null)
        {
            // Actualiza los campos
            result.Title = newData.Title;
            result.Year = newData.Year;
            result.Description = newData.Description;
        }

        return await Task.FromResult(result); // Retorna la película actualizada (o null si no existe)
    }

    // Elimina una película de la base de datos
    public async Task<Movie?> DeleteMovie(int id)
    {
        Movie? result = db.Movies.FirstOrDefault(m => m.Id == id); // Busca la película

        if (result != null) { db.Movies.Remove(result); } // Si existe, elimina

        return await Task.FromResult(result); // Retorna la película eliminada (o null si no existe)
    }
}