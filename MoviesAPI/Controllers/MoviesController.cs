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

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private readonly string containerName = "movies";

        public MoviesController(ApplicationDbContext context, IMapper mapper, IFileStorageService fileStorageService)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
        }
        [HttpGet("allMovies")]
        public async Task<ActionResult<List<MovieDTO>>> Get()
        {
            var movies = await context.Movies.ToListAsync();
            return mapper.Map<List<MovieDTO>>(movies);
        }
        [HttpGet]
        public async Task<ActionResult<List<MovieDTO>>> Get([FromQuery] PaginationDTO paginationDTO) {
            var queryable = context.Movies.AsQueryable();
            await HttpContext.InsertPaginationParams(queryable, paginationDTO.regsPerPage);
            var entities = await queryable.Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<MovieDTO>>(entities);
        }
        
        [HttpGet("{id:int}", Name = "GetMovieById")]
        public async Task<ActionResult<MovieDTO>> Get(int id)
        {
            var movie = await context.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if (movie == null) return NotFound();
            return mapper.Map<MovieDTO>(movie);
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movie = mapper.Map<MovieDTO>(movieCreationDTO);
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
            context.Add(movie);
            await context.SaveChangesAsync();
            var movieDTO = mapper.Map<MovieDTO>(movie);
            return new CreatedAtRouteResult("GetMovieById", new { id = movieDTO.Id }, movieDTO);
        }
        [HttpPut]
        public async Task<ActionResult> Put(int id, [FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movieDB = await context.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if (movieDB == null) return NotFound();
            movieDB = mapper.Map(movieCreationDTO, movieDB);
            if (movieCreationDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreationDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreationDTO.Poster.FileName);

                    movieDB.PosterUrl = await fileStorageService.EditFile(content, extensio n, containerName, movieDB.PosterUrl, movieCreationDTO.Poster.ContentType);
                }
            }
            await context.SaveChangesAsync();
            return NoContent();
        }
        
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await context.Movies.AnyAsync(x => x.Id == id);
            if (!exists) return NotFound();
            context.Remove(new Movie() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<MoviePatchDTO> patchDocument)
        {
            if (patchDocument == null) return BadRequest();
            var entityFromDB = await context.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if (entityFromDB == null) return NotFound();

            var entityDTO = mapper.Map<MoviePatchDTO>(entityFromDB);
            patchDocument.ApplyTo(entityDTO, ModelState);
            var isValid = TryValidateModel(entityDTO);
            if (!isValid) return BadRequest();
            mapper.Map(entityDTO, entityFromDB); ;
            await context.SaveChangesAsync();
            return NoContent();

        }


    } 
}

