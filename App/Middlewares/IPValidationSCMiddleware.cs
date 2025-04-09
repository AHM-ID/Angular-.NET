using System.Net;

namespace Angular.App.Middlewares
{
    public class IPValidationSCMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<IPValidationSCMiddleware> _logger;

        public IPValidationSCMiddleware(
            RequestDelegate next,
            ILogger<IPValidationSCMiddleware> logger
        )
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("IPValidationSCMiddleware executing...");

            try
            {
                if (
                    context.Items.TryGetValue("BlockedIP", out var value)
                    && value is bool isBlockedIP
                )
                {
                    _logger.LogInformation($"Retrieved BlockedIP: {isBlockedIP}");

                    if (isBlockedIP) //! 🚨 Block request if IP is invalid
                    {
                        _logger.LogWarning(
                            "IPValidationSCMiddleware - Blocked IP detected. Blocking request."
                        );

                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden; //! Forbidden
                        context.Response.ContentType = "application/json";
                        var errorResponse = new
                        {
                            status = "error",
                            type = "Access Denied",
                            details = new
                            {
                                message = "Your IP address is not allowed to access this resource.",
                                requestInfo = new
                                {
                                    ipAddress = context.Connection.RemoteIpAddress?.ToString()
                                        ?? "Unknown",
                                    timestamp = DateTime.UtcNow,
                                },
                            },
                            suggestions = new
                            {
                                nextSteps = "If you believe this is a mistake, please contact support.",
                                supportContact = "support@example.com",
                                documentation = "https://example.com/access-policy",
                            },
                        };

                        string errMessage = System.Text.Json.JsonSerializer.Serialize(
                            errorResponse
                        );
                        context.Items["Error"] = errMessage;

                        return; //! 🚨 Stop further execution
                    }
                }
                else
                {
                    _logger.LogError(
                        "IPValidationSCMiddleware - BlockedIP flag is missing or invalid."
                    );

                    var exp = new InvalidOperationException(
                        "IPValidationSCMiddleware not implemented or not setting context.Items properly."
                    );

                    exp.Data["ErrorCode"] = 2001;

                    throw exp;
                }

                //? Pipeline (Browser SC)
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in IPValidationSCMiddleware");
                throw;
            }
        }
    }
}
