using MoviesAPI.Validations;

namespace MoviesAPI.DTOs
{
    public class MovieCreationDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool InCinema { get; set; }
        public DateTime ReleaseDate { get; set; }
        [FileTypeValidation(fileTypeGroup: FileTypeGroup.Image)]
        [FileWeightValidation(MaxWeightInMB: 4)]
        public IFormFile Poster { get; set; }
    }
}
