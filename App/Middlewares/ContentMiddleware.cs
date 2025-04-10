using System.Net;
using System.Text.Json;

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

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("ContentMiddleware executing...");

            try
            {
                var path = context.Request.Path.Value;
                if (
                    !string.IsNullOrEmpty(path)
                    && path.StartsWith("/api", StringComparison.OrdinalIgnoreCase)
                )
                {
                    var originalBodyStream = context.Response.Body;
                    await using var responseBody = new MemoryStream();
                    context.Response.Body = responseBody;

                    try
                    {
                        await _next(context); // Let controller write the actual response

                        responseBody.Seek(0, SeekOrigin.Begin);
                        var responseContent = await new StreamReader(responseBody).ReadToEndAsync();

                        // Store only
                        context.Items["Content"] = responseContent;
                        _logger.LogInformation(
                            "Captured API response content: {Content}",
                            responseContent
                        );
                    }
                    finally
                    {
                        context.Response.Body = originalBodyStream;
                    }
                }
                else if (
                    context.Request.Path.StartsWithSegments("/swagger")
                    || context.Request.Path.StartsWithSegments("/favicon.ico")
                    || context.Request.Path.Value?.Contains(".js") == true
                    || context.Request.Path.Value?.Contains(".css") == true
                )
                {
                    await _next(context);
                    return;
                }
                else
                {
                    // Handle non-API requests
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.ContentType = "application/json";

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
                                ipAddress = context.Connection.RemoteIpAddress?.ToString()
                                    ?? "Unknown",
                            },
                        },
                        metadata = new
                        {
                            source = "ContentMiddleware",
                            description = "This response is generated for supported clients.",
                        },
                    };

                    string jsonContent = JsonSerializer.Serialize(content);
                    _logger.LogInformation("Writing response content: {Content}", jsonContent);

                    // Store in context.Items
                    context.Items["Content"] = jsonContent;
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred in ContentMiddleware while processing the response."
                );

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var errorContent = new
                {
                    error = "An error occurred while processing your request.",
                    details = ex.Message,
                    timestamp = DateTime.UtcNow,
                };

                string jsonErrorContent = JsonSerializer.Serialize(errorContent);
                context.Items["Content"] = jsonErrorContent;

                // Write the error response
                await context.Response.WriteAsync(jsonErrorContent);
            }
        }
    }
}
