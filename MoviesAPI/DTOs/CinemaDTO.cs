using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class CinemaDTO
    {
        public int ID { get; set; }
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
    }
}
