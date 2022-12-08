using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using System.Security.Claims;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace MoviesAPI.Controllers
{
    [Route("/api/movies/{movieId:int}/reviews")]
    [ServiceFilter(typeof(MovieExistsAttribute))]
    public class ReviewController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ReviewController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<List<ReviewDTO>>> Get(int movieId, [FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Reviews.Include(x => x.User).AsQueryable();
            queryable = queryable.Where(x => x.MovieId == movieId);
            return await Get<Review, ReviewDTO>(paginationDTO, queryable);
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult>Post(int movieId, [FromBody]ReviewCreationDTO reviewCreationDTO)
        {
            
            var userID = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var reviewExists = await context.Reviews.AnyAsync(x => x.MovieId == movieId && x.UserId == userID);
            if (reviewExists) return BadRequest("User has already reviewed this movie");
            var review = mapper.Map<Review>(reviewCreationDTO);
            review.MovieId = movieId;
            review.UserId = userID;
            context.Add(review);
            await context.SaveChangesAsync();
            return NoContent();

        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put(int movieId,int reviewId, 
            [FromBody] ReviewCreationDTO reviewCreationDTO)
        {
            var reviewDb = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
            if (reviewDb == null) return NotFound();
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (reviewDb.UserId != userId) return Forbid();
            reviewDb = mapper.Map(reviewCreationDTO, reviewDb);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
