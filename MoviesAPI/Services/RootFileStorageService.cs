namespace MoviesAPI.Services
{
    public class RootFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment environment;
        private readonly IHttpContextAccessor httpContextAccessor;

        public RootFileStorageService(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            this.environment = environment;
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task DeleteFile(string route, string container)
        {
            if (String.IsNullOrEmpty(route))
            {
                var fileName = Path.GetFileName(route);
                string fileDirectory = Path.Combine(environment.WebRootPath, container, fileName);
                if (File.Exists(fileDirectory)) File.Delete(fileDirectory);
                
            }
            return Task.FromResult(0);
        }

        public async Task<string> EditFile(byte[] content, string extension, string container, string route, string contentType)
        {
            await DeleteFile(route, container);
            return await SaveFile(content, extension, container, contentType);
        }

        public async Task<string> SaveFile(byte[] content, string extension, string container, string contentType)
        {
            var fileName = $"{Guid.NewGuid()}.{extension}";
            string folder = Path.Combine(environment.WebRootPath, container);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            string route = Path.Combine(folder, fileName);
            await File.WriteAllBytesAsync(route, content);
            var actualUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var bdUrl = Path.Combine(actualUrl, container, fileName).Replace("\\", "/");
            return bdUrl;
        }
    }
}
