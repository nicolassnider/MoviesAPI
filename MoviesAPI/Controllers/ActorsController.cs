using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    public class ActorsController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private readonly string containerName = "actors";

        public ActorsController(
            ApplicationDbContext context, 
            IMapper mapper, 
            IFileStorageService fileStorageService
            ):base(context,mapper)
        {
            this.fileStorageService = fileStorageService;
        }
        [HttpGet("allActors")]
        public async Task<ActionResult<List<ActorDTO>>> Get()
        {
            return await Get<Actor, ActorDTO>();
        }
        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            return await Get<Actor, ActorDTO>(paginationDTO);
        }
        [HttpGet("{id:int}", Name = "GetActorById")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            return await Get<Actor, ActorDTO>(id);
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreationDTO actorCreationDTO)
        {
            var actor = mapper.Map<Actor>(actorCreationDTO);
            if (actorCreationDTO.Picture!=null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await actorCreationDTO.Picture.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(actorCreationDTO.Picture.FileName);
                    actor.PictureUrl = await fileStorageService.SaveFile(content, extension, containerName, actorCreationDTO.Picture.ContentType);
                }
            }
            context.Add(actor);
            await context.SaveChangesAsync();
            var actorDTO = mapper.Map<ActorDTO>(actor);
            return new CreatedAtRouteResult("GetActorById", new { id = actor.Id }, actorDTO);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreationDTO actorCreationDTO)
        {
            var actorDB = await context.Actors.FirstOrDefaultAsync(x => x.Id == id);
            if (actorDB == null) return NotFound();
            actorDB = mapper.Map(actorCreationDTO, actorDB);
            if (actorCreationDTO.Picture!=null)
            {
                using (var memoryStream = new MemoryStream()) {
                    await actorCreationDTO.Picture.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(actorCreationDTO.Picture.FileName);
                    
                    actorDB.PictureUrl = await fileStorageService.EditFile(content, extension, containerName,actorDB.PictureUrl, actorCreationDTO.Picture.ContentType);
                }
            }
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Actor>(id);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id,[FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument)
        {
            return await Patch<Actor, ActorPatchDTO>(id, patchDocument);
        }

    }
}
