namespace MoviesAPI.DTOs
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;
        public int regsPerPage =10;
        private readonly int maxRegsPerPage = 50;
        public int RegsPerPage
        {
            get => regsPerPage;
            set
            {
                regsPerPage = (value > regsPerPage) ? maxRegsPerPage : value;
            }
        }
    }
}
