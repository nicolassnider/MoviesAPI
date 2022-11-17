using Microsoft.EntityFrameworkCore;

namespace MoviesAPI.Helpers
{
    public static class HttpContextExtensions
    {
        public async static Task InsertPaginationParams<T>(this HttpContext httpContext, IQueryable<T> queriable, int regsPerPage)
        {
            double q = await queriable.CountAsync();
            double qPages = Math.Ceiling(q / regsPerPage);
            httpContext.Response.Headers.Add("pages", qPages.ToString());
        }
    }
}
