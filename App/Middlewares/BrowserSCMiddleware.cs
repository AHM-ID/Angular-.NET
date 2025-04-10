using System.Net;

namespace Angular.App.Middlewares
{
    public class BrowserSCMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<BrowserSCMiddleware> _logger;

        public BrowserSCMiddleware(RequestDelegate next, ILogger<BrowserSCMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("BrowserSCMiddleware executing...");

            try
            {
                if (
                    context.Items.TryGetValue("InvalidBrowser", out var value)
                    && value is bool isInvalidBrowser
                )
                {
                    _logger.LogInformation($"Retrieved InvalidBrowser: {isInvalidBrowser}");

                    if (isInvalidBrowser) //! 🚨 Invalid Browser detected
                    {
                        _logger.LogWarning("Invalid Browser detected - Blocking request.");

                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest; //! Bad Request
                        context.Response.ContentType = "application/json";
                        var errorResponse = new
                        {
                            status = "error",
                            type = "Unsupported Browser",
                            details = new
                            {
                                message = "Your Browser is not supported by this application.",
                                browserInfo = new
                                {
                                    userAgent = context.Request.Headers["User-Agent"].ToString(),
                                    detectedBrowser = context
                                        .Request.Headers["User-Agent"]
                                        .ToString()
                                        .ToLower()
                                        .Contains("firefox")
                                        ? "Firefox"
                                        : "Unknown",
                                },
                                requestInfo = new
                                {
                                    ipAddress = context.Connection.RemoteIpAddress?.ToString()
                                        ?? "Unknown",
                                    timestamp = DateTime.UtcNow,
                                },
                            },
                            suggestions = new
                            {
                                alternativeBrowsers = new[]
                                {
                                    "Google Chrome",
                                    "Microsoft Edge",
                                    "Safari",
                                    "Brave",
                                },
                                supportContact = "support@example.com",
                                documentation = "https://example.com/browser-support",
                            },
                        };

                        string errMessage = System.Text.Json.JsonSerializer.Serialize(
                            errorResponse
                        );
                        context.Items["Error"] = errMessage;

                        return; //! 🚨 STOP further execution
                    }
                }
                else
                {
                    _logger.LogError(
                        "BrowserSCMiddleware - InvalidBrowser flag is missing or invalid."
                    );

                    var exp = new InvalidOperationException(
                        "BrowserSCMiddleware not implemented or not setting context.Items properly."
                    );

                    exp.Data["ErrorCode"] = 1001;

                    throw exp;
                }

                //? Pipeline (Content Generation)
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in BrowserSCMiddleware");
                throw;
            }
        }
    }
}
