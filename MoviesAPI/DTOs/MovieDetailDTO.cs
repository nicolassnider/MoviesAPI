namespace MoviesAPI.DTOs
{
    public class MovieDetailDTO:MovieDTO
    {
        public List<GenreDTO> Genres { get; set; }
        public List<ActorMovieDetailDTO> Actors { get; set; }
    }
}
