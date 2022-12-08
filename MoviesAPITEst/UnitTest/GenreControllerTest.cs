using MoviesAPI.Controllers;
using MoviesAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPITEst.UnitTest
{
    [TestClass]
    internal class GenreControllerTest:BaseTest
    {
        [TestMethod]
        public async Task GetAllGenres()
        {
            //prep
            var nameDb = Guid.NewGuid().ToString();
            var context = BuildContext(nameDb);
            var mapper = ConfigureAutoMapper();

            context.Genres.Add(new Genre() { Name = "Genre 1" });
            context.Genres.Add(new Genre() { Name = "Genre 2" });
            await context.SaveChangesAsync();

            var context2 = BuildContext(nameDb);

            //test
            var controller = new GenresController(context2, mapper);
            var response = await controller.Get();
            //verification
            var genres = response.Value;
            Assert.AreEqual(2, genres.Count);
        }
    }
}
