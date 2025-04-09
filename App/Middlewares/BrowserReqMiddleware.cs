using Angular.App.Services.Contracts;

namespace Angular.App.Middlewares
{
    public class BrowserReqMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<BrowserReqMiddleware> _logger;
        private readonly IBrowserCheckService _browserCheckServices;

        public BrowserReqMiddleware(
            RequestDelegate next,
            ILogger<BrowserReqMiddleware> logger,
            IBrowserCheckService browserCheckService
        )
        {
            _next = next;
            _logger = logger;
            _browserCheckServices = browserCheckService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("BrowserReqMiddleware executing...");

            try
            {
                bool isValidBrowser = false;

                //! Vendor/Version
                if (context.Request.Headers.TryGetValue("User-Agent", out var userAgent))
                {
                    //? Check for valid browser
                    isValidBrowser = await _browserCheckServices.ValidateBrowser(userAgent);

                    _logger.LogInformation(
                        "User-Agent detected: {UserAgent}",
                        userAgent.ToString()
                    );
                }

                _logger.LogInformation("Setting InvalidBrowser: {isValidBrowser}", isValidBrowser);

                //? Ensure ValidBrowser is always set
                context.Items["ValidBrowser"] = isValidBrowser;

                //? Pipeline (IPValidation SC)
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in BrowserReqMiddleware");
                throw;
            }
        }
    }
}
