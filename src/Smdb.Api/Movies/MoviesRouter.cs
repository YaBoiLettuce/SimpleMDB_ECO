namespace Smdb.Api.Movies;

using Shared.Http;

public class MoviesRouter : HttpRouter
{
    public MoviesRouter(MoviesController moviesController)
    {
        UseParametrizedRouteMatching();
        // GET /api/v1/movies/
        MapGet("/", moviesController.ReadMovies);
        //POST /api/v1/movies/
        MapPost("/", HttpUtils.ReadRequestBodyAsText, moviesController.CreateMovie);
        // GET /api/v1/movies/1
        MapGet("/:id", moviesController.ReadMovie);
        //PUT /api/v1/movies/1
        MapPut("/:id", HttpUtils.ReadRequestBodyAsText,
          moviesController.UpdateMovie);
        //DELETE /api/v1/movies/1
        MapDelete("/:id", moviesController.DeleteMovie);
    }

}