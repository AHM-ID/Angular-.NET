using System.Net;
using System.Text.Json;

namespace Angular.App.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger,
            IWebHostEnvironment environment
        )
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                //* Continue with the next middleware in the pipeline
                await _next(context);
            }
            catch (Exception ex) //! Catch any unhandled exception
            {
                int errorCode =
                    ex.Data.Contains("ErrorCode") && ex.Data["ErrorCode"] is int code ? code : 5000; //! Default error code

                _logger.LogError(
                    ex,
                    "Unhandled exception occurred. Error Code: {ErrorCode}",
                    errorCode
                );

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                //! Generate the error response
                var errorResponse = new
                {
                    status = "error",
                    errorCode = errorCode,
                    message = "An unexpected error occurred.",
                    details = _environment.IsDevelopment()
                        ? ex.ToString()
                        : "Please contact support for more information.",
                    timestamp = DateTime.UtcNow,
                };

                // Serialize and write the response
                var jsonResponse = JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
