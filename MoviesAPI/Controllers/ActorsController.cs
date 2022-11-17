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
    [ApiController]
    [Route("api/[controller]")]
    public class ActorsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private readonly string containerName = "actors";

        public ActorsController(ApplicationDbContext context, IMapper mapper, IFileStorageService fileStorageService)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
        }
        [HttpGet("allActors")]
        public async Task<ActionResult<List<ActorDTO>>> Get()
        {
            var entities = await context.Actors.ToListAsync();
            var dtos = mapper.Map<List<ActorDTO>>(entities);
            return dtos;
        }
        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Actors.AsQueryable();
            await HttpContext.InsertPaginationParams(queryable, paginationDTO.regsPerPage);
            var entities = await queryable.Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<ActorDTO>>(entities);
        }
        [HttpGet("{id:int}", Name = "GetActorById")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            var entity = await context.Actors.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return NotFound();
            return mapper.Map<ActorDTO>(entity);
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
            var exists = await context.Actors.AnyAsync(x => x.Id == id);
            if (!exists) return NotFound();
            context.Remove(new Actor() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id,[FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument)
        {
            if (patchDocument == null) return BadRequest();
            var entityFromDB = await context.Actors.FirstOrDefaultAsync(x => x.Id == id);
            if (entityFromDB == null) return NotFound();

            var entityDTO = mapper.Map<ActorPatchDTO>(entityFromDB);
            patchDocument.ApplyTo(entityDTO, ModelState);
            var isValid = TryValidateModel(entityDTO);
            if (!isValid) return BadRequest(ModelState);
            mapper.Map(entityDTO, entityFromDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}
