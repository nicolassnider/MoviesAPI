using MoviesAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class ActorCreationDTO:ActorPatchDTO
    {
        
        [FileTypeValidation(fileTypeGroup:FileTypeGroup.Image)]
        [FileWeightValidation(MaxWeightInMB: 4)]
        public IFormFile Picture { get; set; }
        

    }
}
