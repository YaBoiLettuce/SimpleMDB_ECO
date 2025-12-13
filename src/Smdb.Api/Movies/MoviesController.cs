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