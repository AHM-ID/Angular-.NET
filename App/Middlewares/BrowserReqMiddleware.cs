using Angular.App.Services.Contracts;

namespace Angular.App.Middlewares
{
    public class BrowserReqMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<BrowserReqMiddleware> _logger;
        private readonly IBrowserCheckService _browserCheckService;

        public BrowserReqMiddleware(
            RequestDelegate next,
            ILogger<BrowserReqMiddleware> logger,
            IBrowserCheckService browserCheckService
        )
        {
            _next = next;
            _logger = logger;
            _browserCheckService = browserCheckService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("BrowserReqMiddleware executing...");

            try
            {
                bool isValidBrowser = false;

                //! Vendor/Version check using the "User-Agent" header
                if (context.Request.Headers.TryGetValue("User-Agent", out var userAgent))
                {
                    //? Ensure the userAgent is passed in the correct format (StringValues) for ValidateBrowser method
                    isValidBrowser = await _browserCheckService.ValidateBrowser(userAgent);

                    _logger.LogInformation(
                        "User-Agent detected: {UserAgent}",
                        userAgent.ToString()
                    );
                }

                //? Log the result of browser validation (whether the browser is valid or invalid)
                _logger.LogInformation(
                    "Setting InvalidBrowser flag: {InvalidBrowser}",
                    !isValidBrowser
                );

                //? Store the result of browser validation in the HttpContext.Items
                context.Items["InvalidBrowser"] = !isValidBrowser;

                //? Continue the request pipeline (next middleware)
                await _next(context);
                return;
            }
            catch (Exception ex)
            {
                //? Log any exceptions that occur in the middleware
                _logger.LogError(ex, "Exception in BrowserReqMiddleware");
                throw;
            }
        }
    }
}
