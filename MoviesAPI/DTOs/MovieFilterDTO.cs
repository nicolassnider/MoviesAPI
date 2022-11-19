namespace MoviesAPI.DTOs
{
    public class MovieFilterDTO
    {
        public int Page { get; set; } = 1;
        public int RegsPerPage { get; set; } = 10;
        public PaginationDTO PaginationDTO { get { return new PaginationDTO() { Page = Page, RegsPerPage = RegsPerPage }; } }

        public string Title { get; set; }
        public int GenreId { get; set; }
        public bool InCinema { get; set; }
        public bool NextReleases { get; set; }
        public string OrderBy { get; set; }
        public bool OrderByAscending { get; set; } = true;

    }
}
