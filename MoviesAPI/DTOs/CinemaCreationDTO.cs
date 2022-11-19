﻿using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class CinemaCreationDTO
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
    }
}
