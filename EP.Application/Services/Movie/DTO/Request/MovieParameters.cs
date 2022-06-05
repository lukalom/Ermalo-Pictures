namespace EP.Application.Services.Movie.DTO.Request
{
    public record MovieParameters
    {
        public int PageSize { get; set; }
        public int Page { get; set; }
    }
}
