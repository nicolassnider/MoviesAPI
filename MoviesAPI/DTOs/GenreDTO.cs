using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class GenreDTO:GenreCreationDTO
    { 
        public int Id { get; set; }
        
    }
}
