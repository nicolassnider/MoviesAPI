using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomBaseController:ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        
        

        public CustomBaseController(
            ApplicationDbContext context,
            IMapper mapper
            )
        {
            this.context = context;
            this.mapper = mapper;
        }
        protected async Task<List<TDTO>> Get<TEntity,TDTO>() where TEntity:class
        {
            var entities = await context.Set<TEntity>().AsNoTracking().ToListAsync();
            return mapper.Map<List<TDTO>>(entities);
            
        }
        protected async Task<List<TDTO>> Get<TEntity,TDTO>(PaginationDTO paginationDTO) where TEntity : class
        {
            var queryable = context.Set<TEntity>().AsQueryable();
            await HttpContext.InsertPaginationParams(queryable, paginationDTO.regsPerPage);
            var entities = await queryable.Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<TDTO>>(entities);
        }
        protected async Task<ActionResult <TDTO>> Get<TEntity,TDTO>(int id) where TEntity : class, IId
        {
            var entity = await context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x=>x.Id==id);
            if (entity == null) return NotFound();
            return mapper.Map<TDTO>(entity);
        }
        protected async Task<ActionResult> Post<TCreationDTO,TEntity,TReading>
            (TCreationDTO creationDTO,string routeName) where TEntity : class,IId
        {
            var entity = mapper.Map<TEntity>(creationDTO);
            context.Add(entity);
            await context.SaveChangesAsync();
            var readingDTO = mapper.Map<TReading>(entity);
            return new CreatedAtRouteResult(routeName, new { id = entity.Id }, readingDTO);
        }
        protected async Task<ActionResult> Put<TCreationDTO, TEntity>
            (int id, TCreationDTO creationDTO) where TEntity : class, IId
        {
            var entity = mapper.Map<TEntity>(creationDTO);
            entity.Id = id;
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }
        protected async Task<ActionResult> Delete<TEntity>(int id) where TEntity : class, IId,new()
        {
            var exists = await context.Set<TEntity>().AnyAsync(x => x.Id == id);
            if (!exists) return NotFound();
            context.Remove(new TEntity() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
        protected async Task<ActionResult> Patch
            <TEntity, TPAtchEntityDTO>(
            int id, 
            JsonPatchDocument<TPAtchEntityDTO> patchDocument) 
            where TPAtchEntityDTO:class
            where TEntity:class, IId
        {
            if (patchDocument == null) return BadRequest();
            var entityFromDB = await context.Actors.FirstOrDefaultAsync(x => x.Id == id);
            if (entityFromDB == null) return NotFound();

            var entityDTO = mapper.Map<TPAtchEntityDTO>(entityFromDB);
            patchDocument.ApplyTo(entityDTO, ModelState);
            var isValid = TryValidateModel(entityDTO);
            if (!isValid) return BadRequest(ModelState);
            mapper.Map(entityDTO, entityFromDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
