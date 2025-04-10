using Angular.App.Services.Contracts;

namespace Angular.App.Middlewares
{
    public class IPValidationReqMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IIPValidationService _ipValidationService;
        private readonly ILogger<IPValidationReqMiddleware> _logger;

        public IPValidationReqMiddleware(
            RequestDelegate next,
            IIPValidationService ipValidationService,
            ILogger<IPValidationReqMiddleware> logger
        )
        {
            _next = next;
            _ipValidationService = ipValidationService;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("IPValidationReqMiddleware executing...");

            try
            {
                var remoteIp = context.Connection.RemoteIpAddress;
                if (remoteIp == null)
                {
                    _logger.LogWarning("Remote IP address is null.");
                    context.Items["BlockedIP"] = true;
                    //? Pipeline (Browser Req)
                    await _next(context);
                    return;
                }

                //? Check against allowed IPs
                bool isAllowed = await _ipValidationService.ValidateIP(remoteIp);
                context.Items["BlockedIP"] = !isAllowed;

                //? Pipeline (Browser Req)
                await _next(context);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in IPValidationReqMiddleware");
                throw;
            }
        }
    }
}
