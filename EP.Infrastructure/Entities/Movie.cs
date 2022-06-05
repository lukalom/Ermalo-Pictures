using System.ComponentModel.DataAnnotations;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EP.Infrastructure.Entities
{
    public class Movie : BaseEntity<int>
    {
        public Movie() => Movie_Genres = new List<Movie_Genre>();

        [Required]
        [MaxLength(150, ErrorMessage = "Max Length 150 character")]
        public string Title { get; set; }

        [Required]
        [MaxLength(150, ErrorMessage = "Max Length 150 character")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Duration In Minutes")]
        [Range(30, 300, ErrorMessage = "Minutes should be between 30 and 300")]
        public int DurationInMinutes { get; set; }

        [Required]
        public Language Language { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string Director { get; set; }

        [ValidateNever]
        public string? ImageUrl { get; set; }

        public List<Movie_Genre> Movie_Genres { get; set; }
    }
}
