using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EP.Infrastructure.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EP.Application.Services.Movie.DTO.Request
{
    public class MovieUploadRequestDto
    {
        public MovieUploadRequestDto()
        {
            GenreIdList = new List<int>();
        }

        [Required]
        [MaxLength(150, ErrorMessage = "Max Length 150 character")]
        public string Title { get; set; }

        [Required]
        [MaxLength(150, ErrorMessage = "Max Length 150 character")]
        public string Description { get; set; }

        [Required]
        [Range(30, 300, ErrorMessage = "Minutes should be between 30 and 300")]
        public int DurationInMinutes { get; set; }

        [Required]
        public Language Language { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public string Country { get; set; }

        public List<int> GenreIdList { get; set; }

        [Required]
        public string Director { get; set; }

        public IFormFile? File { get; set; }
    }
}
