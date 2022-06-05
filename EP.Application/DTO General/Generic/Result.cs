using EP.Application.DTO_General.Errors;

namespace EP.Application.DTO_General.Generic
{
    public class Result<T>
    {
        public T Content { get; set; }

        public Error Error { get; set; }

        public bool IsSuccess => Error == null;

        public DateTime ResponseTime { get; set; } = DateTime.UtcNow;
    }

}
