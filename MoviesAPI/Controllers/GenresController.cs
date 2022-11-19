using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;

namespace MoviesAPI.Controllers
{
    public class GenresController : CustomBaseController
    {
        public GenresController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            
        }

        [HttpGet]
        public async Task<ActionResult<List<GenreDTO>>> Get()
        {
            return await Get<Genre, GenreDTO>();
        }
        
        [HttpGet("{id:int}", Name = "GetGenreById")]
        public async Task<ActionResult<GenreDTO>> Get(int id)
        {
            return await Get<Genre,GenreDTO>(id);
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GenreCreationDTO genreCreationDTO)
        {
            return await Post<GenreCreationDTO, Genre, GenreDTO>(genreCreationDTO, "GetGenreById");
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] GenreCreationDTO genreCreationDTO)
        {
            return await Put<GenreCreationDTO, Genre>(id, genreCreationDTO);
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Genre>(id);
        }
    }
}
