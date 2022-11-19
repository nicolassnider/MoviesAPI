using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entities
{
    public class Actor:IId
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PictureUrl { get; set; }
        public List<MoviesActors> MoviesActors { get; set; }
    }
}
