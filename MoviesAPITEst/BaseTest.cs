using MoviesAPI;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite;
using MoviesAPI.Helpers;

namespace MoviesAPITEst
{
    internal class BaseTest
    {
        protected ApplicationDbContext BuildContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName).Options;
            var dbContext = new ApplicationDbContext(options);
            return dbContext;
        }
        protected IMapper ConfigureAutoMapper()
        {
            var config = new MapperConfiguration(opts =>
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                opts.AddProfile(new AutomapperProfiles(geometryFactory));
            });
            return config.CreateMapper();
        }
    }
}
