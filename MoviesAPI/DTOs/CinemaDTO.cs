using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class CinemaDTO
    {
        public int ID { get; set; }
        
        public string Name { get; set; }

        public double Latitude { get; set; } //en x

        public double Longitude { get; set; } //en y
    }
}
