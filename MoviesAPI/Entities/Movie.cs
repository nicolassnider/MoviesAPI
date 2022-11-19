using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entities
{
    public class Movie:IId
    {
        public int Id { get; set; }
        [Required]
        [StringLength(300)]
        public string Title { get; set; }
        public bool InCinema { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string PosterUrl { get; set; }
        public List<MoviesActors> MoviesActors { get; set; }
        public List<MoviesGenres> MoviesGenres { get; set; }
        public List<MoviesCinemas> MoviesCinemas { get; set; }
        
    }
}
