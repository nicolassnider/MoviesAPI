using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using MoviesAPI.Helpers;
using MoviesAPI.Services;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text;
using System.Text.Json.Serialization;

namespace MoviesAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
            //services.AddTransient<IFileStorageService, AzureFileStorageService>();
            //Almacenamiento local
            services.AddTransient<IFileStorageService, RootFileStorageService>();
            services.AddHttpContextAccessor();
            //Fin almacenamiento local
            services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4323));
            services.AddScoped<MovieExistsAttribute>();
            services.AddSingleton(provider =>

                new MapperConfiguration(config =>
                {
                    var geometryFactory = provider.GetRequiredService<GeometryFactory>();
                    config.AddProfile(new AutomapperProfiles(geometryFactory));
                }).CreateMapper()
            );
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptions=>sqlServerOptions.UseNetTopologySuite()
                    );
                
            });
            services.AddControllers().AddNewtonsoftJson();
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Configuration["jwt:key"])),
                    ClockSkew = TimeSpan.Zero
                });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

    }
}
