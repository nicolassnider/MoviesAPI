using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using MoviesAPI.Services;
using System.Reflection.Metadata.Ecma335;
using System.Linq.Dynamic.Core;

namespace MoviesAPI.Controllers
{
    public class MoviesController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private readonly ILogger<MoviesController> logger;
        private readonly string containerName = "movies";

        public MoviesController(
            ApplicationDbContext context, 
            IMapper mapper, 
            IFileStorageService fileStorageService, 
            ILogger<MoviesController> logger
            ):base(context,mapper)
        {
            this.fileStorageService = fileStorageService;
            this.logger = logger;
        }
        [HttpGet("MoviesIndex")]
        public async Task<ActionResult<MoviesIndexDTO>> Get()
        {
            var top = 5;
            var today = DateTime.Today;
            var nextReleases = await context.Movies
                .Where(x => x.ReleaseDate > today)
                .OrderBy(x => x.ReleaseDate)
                .Take(top).ToListAsync();
            var inCinema = await context.Movies
                .Where(x => x.InCinema)
                .Take(top)
                .ToListAsync();
            var result = new MoviesIndexDTO();
            result.NextReleases = mapper.Map<List<MovieDTO>>(nextReleases);
            result.InCinema = mapper.Map<List<MovieDTO>>(inCinema);
            return result;
        }
        [HttpGet]
        public async Task<ActionResult<List<MovieDTO>>> Get([FromQuery] PaginationDTO paginationDTO) {
            return await Get<Movie, MovieDTO>(paginationDTO);
        }
        [HttpGet("filter")]
        public async Task<ActionResult<List<MovieDTO>>> Filter([FromQuery] MovieFilterDTO movieFilterDTO)
        {
            var moviesQueryable = context.Movies.AsQueryable();
            if (!string.IsNullOrEmpty(movieFilterDTO.Title)) moviesQueryable.Where(x => x.Title.Contains(movieFilterDTO.Title));
            if (movieFilterDTO.InCinema) moviesQueryable.Where(x => x.InCinema);
            if (movieFilterDTO.NextReleases) moviesQueryable.Where(x => x.ReleaseDate > DateTime.Today);
            if (movieFilterDTO.GenreId != 0) moviesQueryable.Where(x => x.MoviesGenres.Select(y => y.GenreId).Contains(movieFilterDTO.GenreId));
            if (!string.IsNullOrEmpty(movieFilterDTO.OrderBy))
            {
                try
                {
                    var order = movieFilterDTO.OrderByAscending ? "ascending" : "descending";
                    moviesQueryable = moviesQueryable.OrderBy($"{movieFilterDTO.OrderBy} {order}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message,ex);
                }
                
            }
            await HttpContext.InsertPaginationParams(moviesQueryable, movieFilterDTO.PaginationDTO.RegsPerPage);
            var movies = await moviesQueryable.Paginate(movieFilterDTO.PaginationDTO).ToListAsync();
            return mapper.Map<List<MovieDTO>>(movies);
        }
        

        [HttpGet("{id:int}", Name = "GetMovieById")]
        public async Task<ActionResult<MovieDetailDTO>> Get(int id)
        {
            var movie = await context.Movies
                .Include(x => x.MoviesActors).ThenInclude(x => x.Actor)
                .Include(x => x.MoviesGenres).ThenInclude(x=>x.Genre)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (movie == null) return NotFound();
            movie.MoviesActors = movie.MoviesActors.OrderBy(x => x.Order).ToList();
            return mapper.Map<MovieDetailDTO>(movie);
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movie = mapper.Map<Movie>(movieCreationDTO);
            if (movieCreationDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreationDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreationDTO.Poster.FileName);
                    movie.PosterUrl = await fileStorageService.SaveFile(content, extension, containerName, movieCreationDTO.Poster.ContentType);
                }
            }
            AssingActorOrder(movie);
            context.Add(movie);
            await context.SaveChangesAsync();
            var movieDTO = mapper.Map<MovieDTO>(movie);
            return new CreatedAtRouteResult("GetMovieById", new { id = movieDTO.Id }, movieDTO);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movieDB = await context.Movies
                .Include(x => x.MoviesActors)
                .Include(x => x.MoviesGenres)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (movieDB == null) return NotFound();
            movieDB = mapper.Map(movieCreationDTO, movieDB);
            if (movieCreationDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreationDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreationDTO.Poster.FileName);

                    movieDB.PosterUrl = await fileStorageService.EditFile(content, extension, containerName, movieDB.PosterUrl, movieCreationDTO.Poster.ContentType);
                }
            }
            AssingActorOrder(movieDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
        
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Movie>(id);
        }
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<MoviePatchDTO> patchDocument)
        {
            return await Patch<Movie, MoviePatchDTO>(id, patchDocument);

        }
        private void AssingActorOrder(Movie movie)
        {
            if (movie.MoviesActors !=null)
                for (int i = 0; i < movie.MoviesActors.Count; i++) movie.MoviesActors[i].Order = i;
        }


    } 
}

