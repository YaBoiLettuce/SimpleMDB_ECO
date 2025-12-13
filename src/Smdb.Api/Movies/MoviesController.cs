namespace Smdb.Api.Movies;

using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text.Json;
using Shared.Http;
using Smdb.Core.Movies;

public class MoviesController
{
    private IMovieService movieService;

    public MoviesController(IMovieService movieService)
    {
        this.movieService = movieService;
    }

    // ----------------------------------------------------------------------
    // Endpoint: GET /api/v1/movies
    // curl -X GET "http://localhost:8080/api/v1/movies?page=1&size=10"
    // ----------------------------------------------------------------------
    public async Task ReadMovies(HttpListenerRequest req, HttpListenerResponse res,
      Hashtable props, Func<Task> next)
    {
        // Obtiene los parámetros de paginación de la request (page y size)
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 9;

        // Llama al servicio de películas para obtener la lista paginada
        var result = await movieService.ReadMovies(page, size);

        // Envía la respuesta paginada como JSON usando JsonUtils
        await JsonUtils.SendPagedResultResponse(req, res, props, result, page, size);

        await next();
    }

    // ----------------------------------------------------------------------
    // Endpoint: POST /api/v1/movies
    // curl -X POST "http://localhost:8080/api/v1/movies" 
    // -H "Content-Type: application/json" 
    // -d "{ \"id\": -1, \"title\": \"Inception\", \"year\": 2010, \"description\": \"A skilled thief who enters dreams to steal secrets.\" }"
    // ----------------------------------------------------------------------
    public async Task CreateMovie(HttpListenerRequest req,
    HttpListenerResponse res, Hashtable props, Func<Task> next)
    {
        // Obtiene el cuerpo de la request como texto
        var text = (string)props["req.text"]!;

        // Deserializa el JSON a un objeto Movie
        var movie = JsonSerializer.Deserialize<Movie>(text,
        JsonSerializerOptions.Web);

        // Llama al servicio para crear la película
        var result = await movieService.CreateMovie(movie!);

        // Envía la respuesta con el resultado de la creación
        await JsonUtils.SendResultResponse(req, res, props, result);

        await next();
    }

    // ----------------------------------------------------------------------
    // Endpoint: GET /api/v1/movies/{id}
    // curl -X GET "http://localhost:8080/api/v1/movies/1"
    // ----------------------------------------------------------------------
    public async Task ReadMovie(HttpListenerRequest req, HttpListenerResponse res,
      Hashtable props, Func<Task> next)
    {
        // Obtiene el parámetro 'id' desde la URL
        var uParams = (NameValueCollection)props["req.params"]!;
        int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;

        // Llama al servicio para obtener la película por ID
        var result = await movieService.ReadMovie(id);

        // Envía la respuesta con la película encontrada o error
        await JsonUtils.SendResultResponse(req, res, props, result);

        await next();
    }

    // ----------------------------------------------------------------------
    // Endpoint: PUT /api/v1/movies/{id}
    // curl -X PUT "http://localhost:8080/api/v1/movies/1" 
    // -H "Content-Type: application/json" 
    // -d "{ \"title\": \"Joker 2\", \"year\": 2020, \"description\": \"A man that is a joke.\" }"
    // ----------------------------------------------------------------------
    public async Task UpdateMovie(HttpListenerRequest req,
      HttpListenerResponse res, Hashtable props, Func<Task> next)
    {
        // Obtiene el parámetro 'id' desde la URL
        var uParams = (NameValueCollection)props["req.params"]!;
        int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;

        // Obtiene el cuerpo de la request como texto
        var text = (string)props["req.text"]!;

        // Deserializa el JSON a un objeto Movie
        var movie = JsonSerializer.Deserialize<Movie>(text,
          JsonSerializerOptions.Web);

        // Llama al servicio para actualizar la película
        var result = await movieService.UpdateMovie(id, movie!);

        // Envía la respuesta con el resultado de la actualización
        await JsonUtils.SendResultResponse(req, res, props, result);

        await next();
    }

    // ----------------------------------------------------------------------
    // Endpoint: DELETE /api/v1/movies/{id}
    // curl -X DELETE http://localhost:8080/api/v1/movies/1
    // ----------------------------------------------------------------------
    public async Task DeleteMovie(HttpListenerRequest req,
    HttpListenerResponse res, Hashtable props, Func<Task> next)
    {
        // Obtiene el parámetro 'id' desde la URL
        var uParams = (NameValueCollection)props["req.params"]!;
        int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;

        // Llama al servicio para eliminar la película
        var result = await movieService.DeleteMovie(id);

        // Envía la respuesta con el resultado de la eliminación
        await JsonUtils.SendResultResponse(req, res, props, result);

        await next();
    }

}