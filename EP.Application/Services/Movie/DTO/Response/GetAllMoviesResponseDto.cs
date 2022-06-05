using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EP.Application.DTO_General.Generic;

namespace EP.Application.Services.Movie.DTO.Response
{
    public class GetAllMoviesResponseDto
    {
        public GetAllMoviesResponseDto()
        {
            Movies = new List<GetMovieResponseDto>();
        }

        public MetaData MetaData { get; set; }
        public List<GetMovieResponseDto> Movies { get; set; }

    }
}
