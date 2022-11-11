using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenresController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GenreDTO>>> Get()
        {
            var entities = await context.Genres.ToListAsync();
            var dtos = mapper.Map<List<GenreDTO>>(entities);
            return dtos;
        }
        [HttpGet("{id:int}", Name = "GetGenreById")]
        public async Task<ActionResult<GenreDTO>> Get(int id)
        {
            var entity = await context.Genres.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return NotFound();
            return mapper.Map<GenreDTO>(entity);
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GenreCreationDTO genreCreationDTO)
        {
            var entity = mapper.Map<Genre>(genreCreationDTO);
            context.Add(entity);
            await context.SaveChangesAsync();
            var genreDTO = mapper.Map<GenreDTO>(entity);
            return new CreatedAtRouteResult("GetGenreById", new { id = genreDTO.Id }, genreDTO);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] GenreCreationDTO genreCreationDTO)
        {
            var entity = mapper.Map<Genre>(genreCreationDTO);
            entity.Id = id;
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await context.Genres.AnyAsync(x => x.Id == id);
            if (!exists) return NotFound();
            context.Remove(new Genre() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
