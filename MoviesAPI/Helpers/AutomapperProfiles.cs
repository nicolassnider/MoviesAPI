using AutoMapper;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;

namespace MoviesAPI.Helpers
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<Genre, GenreDTO>().ReverseMap();
            CreateMap<GenreCreationDTO, Genre>();

            CreateMap<Cinema, CinemaDTO>().ReverseMap();
            CreateMap<CinemaCreationDTO, Cinema>();

            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<ActorCreationDTO, Actor>()
                .ForMember(x => x.PictureUrl, options => options.Ignore());
            CreateMap<ActorPatchDTO, Actor>().ReverseMap();

            CreateMap<Movie, MovieDTO>().ReverseMap();
            CreateMap<MovieCreationDTO, Movie>()
                .ForMember(x => x.PosterUrl, options => options.Ignore())
                .ForMember(x => x.MoviesGenres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(x => x.MoviesActors, options => options.MapFrom(MapMoviesActors));
            CreateMap<Movie, MovieDetailDTO>()
                .ForMember(x => x.Genres, options => options.MapFrom(MapMovieGenres))
                .ForMember(x => x.Actors, options => options.MapFrom(MapMovieActors));
            CreateMap<MoviePatchDTO, Movie>().ReverseMap();

            CreateMap<Cinema, CinemaDTO>().ReverseMap();
            CreateMap<CinemaCreationDTO,Cinema >().ReverseMap();

        }


        private List<ActorMovieDetailDTO> MapMovieActors(Movie movie, MovieDetailDTO movieDetailDTO)
        {
            var result = new List<ActorMovieDetailDTO>();
            if (movie.MoviesActors == null) return result;
            foreach (var movieActor in movie.MoviesActors)
            {
                result.Add(new ActorMovieDetailDTO() { ActorID = movieActor.ActorId, Name = movieActor.Actor.Name, Character=movieActor.Character });
            }
            return result;
        }

        private List<GenreDTO> MapMovieGenres(Movie movie, MovieDetailDTO movieDetailDTO)
        {
            var result = new List<GenreDTO>();
            if (movie.MoviesGenres == null) return result;
            foreach (var movieGenre in movie.MoviesGenres) 
            {
                result.Add(new GenreDTO() { Id = movieGenre.GenreId, Name = movieGenre.Genre.Name });
            }
            return result;        
        } 
        private List<MoviesGenres> MapMoviesGenres(MovieCreationDTO movieCreationDTO,Movie movie)
        {
            var result = new List<MoviesGenres>();
            if (movieCreationDTO.GenresIds == null) return result;
            foreach (var id in movieCreationDTO.GenresIds)
            {
                result.Add(new MoviesGenres() { GenreId = id });
            }
            return result;
        }
        private List<MoviesActors> MapMoviesActors(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MoviesActors>();
            if (movieCreationDTO.Actors == null) return result;
            foreach (var actor in movieCreationDTO.Actors)
            {
                result.Add(new MoviesActors() { ActorId = actor.ActorId,Character=actor.Character });
            }
            return result;
        }
    }
}
