using EP.Infrastructure.Entities;
using Xunit;

namespace EP.XUnitTests
{
    public class MovieTests
    {
        [Fact]
        public void Movie_DurationInMinutes_ShouldBeBetween_30And300()
        {
            var movie = new Movie
            {
                Title = "The Batman",
                //DurationInMinutes = 29 //false
                DurationInMinutes = 60
            };

            //movie.DurationInMinutes = 301; //true

            Assert.True(movie.DurationInMinutes is > 30 and < 300);
        }



    }

}