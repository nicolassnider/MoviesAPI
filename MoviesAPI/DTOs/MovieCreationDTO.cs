using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Helpers;
using MoviesAPI.Validations;

namespace MoviesAPI.DTOs
{
    public class MovieCreationDTO:MoviePatchDTO
    {
        
        [FileTypeValidation(fileTypeGroup: FileTypeGroup.Image)]
        [FileWeightValidation(MaxWeightInMB: 4)]
        public IFormFile Poster { get; set; }
        [ModelBinder(BinderType =typeof(TypeBinder<List<int>>))]
        public List<int> GenresIds { get; set; }
        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorMovieCreationDTO>>))]
        public List<ActorMovieCreationDTO> Actors { get; set; }
    }
}
