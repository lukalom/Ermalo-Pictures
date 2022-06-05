using EP.Application.DTO_General.Errors;

namespace EP.Application.DTO_General.Extension
{
    public static class ErrorHandler
    {
        public static Error PopulateError(int code, string message, string type)
        {
            return new Error()
            {
                Code = code,
                Message = message,
                Type = type
            };
        }
    }
}
