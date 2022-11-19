using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;

namespace MoviesAPI.Controllers
{
    public class CinemasController : CustomBaseController
    {
        public CinemasController(ApplicationDbContext context, IMapper mapper, ILogger<CinemasController> logger
            ) : base(context, mapper)
        {
        }
        public async Task<ActionResult<List<CinemaDTO>>> Get()
        {
            return await Get<Cinema, CinemaDTO>();
        }
        [HttpGet("{id:int}", Name = "getCinemaById")]
        public async Task<ActionResult<CinemaDTO>> Get(int id)
        {
            return await Get<Cinema, CinemaDTO>(id);
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CinemaCreationDTO cinemaCreationDTO)
        {
            return await Post<CinemaCreationDTO, Cinema, CinemaDTO>(cinemaCreationDTO, "getCinemaById");
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] CinemaCreationDTO cinemaCreationDTO)
        {
            return await Put<CinemaCreationDTO, Cinema>(id, cinemaCreationDTO);
        }
        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<CinemaPatchDTO> patchDocument)
        {
            return await Patch<Cinema, CinemaPatchDTO>(id, patchDocument);
        }


    }

}
