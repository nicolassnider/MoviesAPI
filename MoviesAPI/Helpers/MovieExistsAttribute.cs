using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MoviesAPI.Helpers
{
    public class MovieExistsAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly ApplicationDbContext dBcontext;

        public MovieExistsAttribute(ApplicationDbContext context)
        {
            this.dBcontext = context;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var movieIdObject = context.HttpContext.Request.RouteValues["movieId"];
            if (movieIdObject==null)
            {
                return;
            }
            var movieId = int.Parse(movieIdObject.ToString());
            var movieExists = await dBcontext.Movies.AnyAsync(x => x.Id == movieId);
            if (!movieExists) { 
                context.Result = new NotFoundResult();
            }
            else
            {
                await next();
            }

        }
    }
}
