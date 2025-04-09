using Angular.App.Options;
using Angular.App.Services.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Angular.App.Services
{
    public class BrowserCheckService : IBrowserCheckService
    {
        private readonly List<string> _invalidBrowsers;
        private readonly ILogger<BrowserCheckService> _logger;

        public BrowserCheckService(
            IOptions<StaticVariablesOptions> options,
            ILogger<BrowserCheckService> logger
        )
        {
            _invalidBrowsers = options.Value.InvalidBrowsersList ?? new List<string>();
            _logger = logger;
        }

        public async Task<bool> ValidateBrowser(StringValues header)
        {
            return await Task.Run(() =>
            {
                //? Check if the header is empty or missing
                if (header.Count == 0)
                {
                    _logger.LogWarning("Browser validation failed: header is empty.");
                    return false;
                }

                //? Determine if any value in the header matches a known invalid browser
                bool hasInvalid = header.Any(val =>
                    !string.IsNullOrWhiteSpace(val)
                    && _invalidBrowsers.Any(invalid =>
                        val.Contains(invalid, StringComparison.OrdinalIgnoreCase)
                    )
                );

                if (hasInvalid)
                {
                    //? Log the failure with the problematic header
                    _logger.LogWarning(
                        "Browser validation failed: invalid browser detected in header [{Header}]",
                        string.Join(", ", header.ToArray())
                    );
                    return false;
                }

                //? Log success if no invalid browsers were found
                _logger.LogInformation("Browser validation succeeded.");
                return true;
            });
        }
    }
}
