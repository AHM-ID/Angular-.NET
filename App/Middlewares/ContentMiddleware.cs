using System.Net;

namespace Angular.App.Middlewares
{
    public class ContentMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ContentMiddleware> _logger;

        public ContentMiddleware(RequestDelegate next, ILogger<ContentMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        //* Version 1.0
        //? Async (eager) Runs immediately after call method
        // public async Task InvokeAsync(HttpContext context)
        // {
        //     await Task.Run(() =>
        //     {
        //         _logger.LogInformation("ContentMiddleware executing...");

        //         context.Response.StatusCode = (int)HttpStatusCode.OK; //* OK
        //         context.Response.ContentType = "application/json";

        //         //? Sample message for non-Mozilla browsers
        //         var content = "{\"message\": \"Sample message for non mozilla browsers!\"}";

        //         _logger.LogInformation("Writing response content: {Content}", content);

        //         context.Items["Content"] = content;
        //     });
        // }

        //* Version 2.0
        //? Async (defer) Runs lazy when it truely needs
        public Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("ContentMiddleware executing...");

            try
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK; //* OK
                context.Response.ContentType = "application/json";

                //? Construct a detailed response message
                var content = new
                {
                    status = "success",
                    type = "Content Response",
                    details = new
                    {
                        message = "Sample message for non-Mozilla browsers.",
                        timestamp = DateTime.UtcNow,
                        requestInfo = new
                        {
                            userAgent = context.Request.Headers["User-Agent"].ToString(),
                            ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                        },
                    },
                    metadata = new
                    {
                        source = "ContentMiddleware",
                        description = "This response is generated for supported clients.",
                    },
                };

                string jsonContent = System.Text.Json.JsonSerializer.Serialize(content);
                _logger.LogInformation("Writing response content: {Content}", jsonContent);

                context.Items["Content"] = jsonContent;

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred in ContentMiddleware while processing the response."
                );
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var errorContent = new
                {
                    error = "An error occurred while processing your request.",
                    details = ex.Message,
                    timestamp = DateTime.UtcNow,
                };
                string jsonErrorContent = System.Text.Json.JsonSerializer.Serialize(errorContent);
                return Task.CompletedTask;
            }
        }
    }
}
