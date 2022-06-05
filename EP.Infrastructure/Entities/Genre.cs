using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EP.Infrastructure.IConfiguration;

namespace EP.Infrastructure.Entities
{
    public class Genre : BaseEntity<int>
    {
        [Required]
        public string Name { get; set; }

        public List<Movie_Genre> Movie_Genres { get; set; }
    }
}
