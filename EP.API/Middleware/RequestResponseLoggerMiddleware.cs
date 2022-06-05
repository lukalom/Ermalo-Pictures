namespace EP.API.Middleware
{
    public class RequestResponseLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public RequestResponseLoggerMiddleware(RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory
                .CreateLogger<RequestResponseLoggerMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context)
        {

            _logger.LogInformation($"***Request Details***{Environment.NewLine}Content Types:" +
                                   $" {context.Request.ContentType}{Environment.NewLine}Scheme:" +
                                   $" {context.Request.Scheme}{Environment.NewLine}" +
                                   $"IP {context.Connection.RemoteIpAddress}");

            await _next(context);

            _logger.LogInformation($"***Response Details***{Environment.NewLine}Content Types:" +
                                   $" {context.Response.ContentType}{Environment.NewLine}Status Code:" +
                                   $" {context.Response.StatusCode}{Environment.NewLine}");
        }

    }
}
