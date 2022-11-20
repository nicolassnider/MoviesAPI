using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using MoviesAPI.Entities;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace MoviesAPI
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MoviesActors>()
                .HasKey(x => new { x.ActorId, x.MovieId });
            modelBuilder.Entity<MoviesGenres>()
                .HasKey(x => new { x.GenreId, x.MovieId });
            modelBuilder.Entity<MoviesCinemas>()
                .HasKey(x => new { x.CinemaId, x.MovieId });

            SeedData(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }
        
        private void SeedData(ModelBuilder modelBuilder)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var cinepolis = new Cinema() { Id = 4, Name = "Cinepolis", Location = geometryFactory.CreatePoint(new Coordinate(-34.44144, -58.87179)) };
            var cinemarkTom = new Cinema() { Id = 5, Name = "Cinemark TOM", Location = geometryFactory.CreatePoint(new Coordinate(-34.45114, -58.72646)) };
            var multiplex = new Cinema() { Id = 6, Name = "Multiplex", Location = geometryFactory.CreatePoint(new Coordinate(-34.44563, -58.86833)) };
            var cinemarkDot = new Cinema() { Id = 7, Name = "Cinemark DOT", Location = geometryFactory.CreatePoint(new Coordinate(-34.54588, -58.48899)) };
            modelBuilder.Entity<Cinema>()
                .HasData(new List<Cinema> { cinepolis, cinemarkTom, multiplex, cinemarkDot });
            var adventure = new Genre() { Id = 4, Name = "Adventure" };
            var animation = new Genre() { Id = 5, Name = "Animation" };
            var suspense = new Genre() { Id = 6, Name = "Suspense" };
            var romance = new Genre() { Id = 7, Name = "Romance" };
            
            modelBuilder.Entity<Genre>()
                .HasData(new List<Genre>{ adventure, animation, suspense, romance });

            var jimCarrey = new Actor() { Id = 5, Name = "Jim Carrey", DateOfBirth = new DateTime(1962, 1, 17) };
            var robertDowney = new Actor() { Id = 6, Name = "Robert Downey Jr", DateOfBirth = new DateTime(1965, 1, 9) };
            var chrisEvans = new Actor() { Id = 7, Name = "Chris Evans", DateOfBirth = new DateTime(1981, 06, 13) };

            modelBuilder.Entity<Actor>()
                .HasData(new List<Actor> { jimCarrey, robertDowney, chrisEvans });

            var endGame = new Movie() { Id = 2, Title = "Avengers Endgame", InCinema = true, ReleaseDate = new DateTime(2022, 11, 15) };
            var infinityWars = new Movie() { Id = 3, Title = "Avengers Infinity Wars", InCinema = false, ReleaseDate = new DateTime(2021, 10, 15) };
            var sonic = new Movie() { Id = 4, Title = "Sonic the Hedgehog", InCinema = false, ReleaseDate = new DateTime(2020, 02, 28) };
            var emma = new Movie() { Id = 5, Title = "Emma", InCinema = false, ReleaseDate = new DateTime(2021, 03, 15) };
            var wonderWoman = new Movie() { Id = 6, Title = "Wonder Woman 1984", InCinema = false, ReleaseDate = new DateTime(2021, 07, 12) };

            modelBuilder.Entity<Movie>()
                .HasData(new List<Movie> { endGame, infinityWars, sonic, emma, wonderWoman });

            modelBuilder.Entity<MoviesGenres>().HasData(
                new List<MoviesGenres>()
                {
                    new MoviesGenres(){MovieId=endGame.Id,GenreId=suspense.Id},
                    new MoviesGenres(){MovieId=endGame.Id,GenreId=adventure.Id},
                    new MoviesGenres(){MovieId=infinityWars.Id,GenreId=suspense.Id},
                    new MoviesGenres(){MovieId=infinityWars.Id,GenreId=adventure.Id},
                    new MoviesGenres(){MovieId=sonic.Id,GenreId=adventure.Id},
                    new MoviesGenres(){MovieId=emma.Id,GenreId=suspense.Id},
                    new MoviesGenres(){MovieId=emma.Id,GenreId=romance.Id},
                    new MoviesGenres(){MovieId=wonderWoman.Id,GenreId=suspense.Id},
                    new MoviesGenres(){MovieId=wonderWoman.Id,GenreId=adventure.Id},
                });
            modelBuilder.Entity<MoviesActors>().HasData(
                new List<MoviesActors>()
                {
                    new MoviesActors(){MovieId=endGame.Id,ActorId=robertDowney.Id,Character="Tony Stark"},
                    new MoviesActors(){MovieId=endGame.Id,ActorId=chrisEvans.Id,Character="Steve Rogers"},
                    new MoviesActors(){MovieId=infinityWars.Id,ActorId=robertDowney.Id,Character="Tony Stark"},
                    new MoviesActors(){MovieId=infinityWars.Id,ActorId=chrisEvans.Id,Character="Steve Rogers"},
                    new MoviesActors(){MovieId=sonic.Id,ActorId=jimCarrey.Id,Character="Dr Robotnic"},
                });
        }
        public DbSet<Genre> Genres{ get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MoviesGenres> MoviesGenres { get; set; }
        public DbSet<MoviesActors> MoviesActors { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<MoviesCinemas> MoviesCinemas { get; set; }
    }
}
