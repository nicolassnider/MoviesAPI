using MoviesAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class ActorCreationDTO
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        [FileTypeValidation(fileTypeGroup:FileTypeGroup.Image)]
        [FileWeightValidation(MaxWeightInMB: 4)]
        public IFormFile Picture { get; set; }
        

    }
}
