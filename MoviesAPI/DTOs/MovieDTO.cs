﻿using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool InCinema { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string PosterUrl { get; set; }
    }
}
