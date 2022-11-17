using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class ActorPatchDTO
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public string DateOfBirth { get; set; }

    }
}
