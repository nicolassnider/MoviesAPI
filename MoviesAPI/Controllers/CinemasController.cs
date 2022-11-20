using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using NetTopologySuite.Geometries;

namespace MoviesAPI.Controllers
{
    public class CinemasController : CustomBaseController
    {
        private readonly GeometryFactory geometryFactory;

        public ApplicationDbContext Context { get; }

        public CinemasController(ApplicationDbContext context, IMapper mapper, ILogger<CinemasController> logger, GeometryFactory geometryFactory
            ) : base(context, mapper)
        {
            Context = context;
            this.geometryFactory = geometryFactory;
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
        [HttpGet("near")]
        public async Task<ActionResult<List<NearestCinemaDTO>>> Near([FromQuery] NearestCinemaFilterDTO nearestCinemaFilterDTO)
        {
            var userLocation = geometryFactory.CreatePoint(new Coordinate(nearestCinemaFilterDTO.Longitude, nearestCinemaFilterDTO.Latitude));

            var cinemas = await Context.Cinemas
                .OrderBy(x => x.Location.Distance(userLocation))
                .Where(x => x.Location.IsWithinDistance(userLocation, nearestCinemaFilterDTO.DistanceInKm * 1000))
                .Select(x => new NearestCinemaDTO
                {
                    ID = x.Id,
                    Name = x.Name,
                    Latitude = x.Location.Y,
                    Longitude = x.Location.X,
                    DistanceInM = Math.Round(x.Location.Distance(userLocation))
                })
                .ToListAsync();

            return cinemas;
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
