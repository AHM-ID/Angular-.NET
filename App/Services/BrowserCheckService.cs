using Angular.App.Services.Contracts;
using Angular.App.Services.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using UAParser;

namespace Angular.App.Services
{
    public class BrowserCheckService : IBrowserCheckService
    {
        private readonly List<BrowserRule> _rules;
        private readonly ILogger<BrowserCheckService> _logger;
        private readonly Parser _uaParser;

        public BrowserCheckService(
            IOptions<BrowserCheckServiceOptions> options,
            ILogger<BrowserCheckService> logger
        )
        {
            _logger = logger;
            _uaParser = Parser.GetDefault();
            _rules = BuildRules(options.Value.InvalidBrowsersList ?? new List<InvalidBrowser>());
            _logger.LogDebug("Initialized with {Count} browser rules", _rules.Count);
        }

        public async Task<bool> ValidateBrowser(StringValues header)
        {
            return await Task.Run(() =>
            {
                if (header.Count == 0)
                {
                    _logger.LogWarning("Browser validation failed: User-Agent header is empty.");
                    return false;
                }

                string combinedHeader = string.Join(" ", header.Where(h => h != null));
                ClientInfo clientInfo = _uaParser.Parse(combinedHeader);
                string browserName = clientInfo.UA.Family;
                string browserVersion = GetBrowserVersion(clientInfo);

                // Ensure browserVersion is not null or empty before parsing
                if (string.IsNullOrWhiteSpace(browserVersion))
                {
                    _logger.LogWarning(
                        "Browser version is null or empty for header: {Header}",
                        combinedHeader
                    );
                    return false; // Fail validation if version is invalid
                }

                if (
                    !Version.TryParse(browserVersion, out Version? userVersion)
                    || userVersion == null
                )
                {
                    _logger.LogWarning(
                        "Failed to parse browser version '{Version}' from header: {Header}",
                        browserVersion,
                        combinedHeader
                    );
                    return false; // Fail validation if parsing fails or version is null
                }

                foreach (var rule in _rules)
                {
                    if (rule.IsMatch(browserName, userVersion))
                    {
                        _logger.LogWarning(
                            "Browser validation failed: Invalid browser {Name} {Version} detected in header [{Header}]",
                            browserName,
                            browserVersion,
                            combinedHeader
                        );
                        return false;
                    }
                }

                _logger.LogInformation(
                    "Browser validation succeeded. Header: {Header}",
                    combinedHeader
                );
                return true;
            });
        }

        private List<BrowserRule> BuildRules(List<InvalidBrowser> invalids)
        {
            var rules = new List<BrowserRule>();

            foreach (var invalid in invalids)
            {
                foreach (var version in invalid.Versions)
                {
                    string trimmedVersion = version.Trim();
                    BrowserRule rule;

                    if (trimmedVersion.StartsWith("<="))
                    {
                        string versionStr = trimmedVersion[2..].Trim();
                        if (Version.TryParse(versionStr, out var maxVersion))
                        {
                            rule = new LessThanOrEqualRule(invalid.Vendor, maxVersion);
                            rules.Add(rule);
                            _logger.LogDebug(
                                "Added <= rule for {Vendor}: {MaxVersion}",
                                invalid.Vendor,
                                maxVersion
                            );
                        }
                        else
                        {
                            _logger.LogWarning(
                                "Failed to parse '<=' version '{Version}' for {Vendor}",
                                trimmedVersion,
                                invalid.Vendor
                            );
                        }
                    }
                    else if (trimmedVersion.EndsWith(".x"))
                    {
                        string versionPrefix = trimmedVersion[..^2];
                        if (int.TryParse(versionPrefix, out var majorVersion))
                        {
                            rule = new RangeRule(invalid.Vendor, majorVersion);
                            rules.Add(rule);
                            _logger.LogDebug(
                                "Added .x rule for {Vendor}: {MajorVersion}.x",
                                invalid.Vendor,
                                majorVersion
                            );
                        }
                        else
                        {
                            _logger.LogWarning(
                                "Failed to parse '.x' version prefix '{Version}' for {Vendor}",
                                trimmedVersion,
                                invalid.Vendor
                            );
                        }
                    }
                    else
                    {
                        if (Version.TryParse(trimmedVersion, out var exactVersion))
                        {
                            rule = new ExactMatchRule(invalid.Vendor, exactVersion);
                            rules.Add(rule);
                            _logger.LogDebug(
                                "Added exact rule for {Vendor}: {ExactVersion}",
                                invalid.Vendor,
                                exactVersion
                            );
                        }
                        else
                        {
                            _logger.LogWarning(
                                "Failed to parse exact version '{Version}' for {Vendor}",
                                trimmedVersion,
                                invalid.Vendor
                            );
                        }
                    }
                }
            }

            return rules;
        }

        private static string GetBrowserVersion(ClientInfo clientInfo)
        {
            // Ensure non-null and valid version string
            var major = string.IsNullOrEmpty(clientInfo.UA.Major) ? "0" : clientInfo.UA.Major;
            var minor = string.IsNullOrEmpty(clientInfo.UA.Minor) ? "0" : clientInfo.UA.Minor;
            return $"{major}.{minor}";
        }
    }

    public abstract class BrowserRule
    {
        protected readonly string Vendor;

        protected BrowserRule(string vendor)
        {
            Vendor = vendor;
        }

        public bool IsMatch(string browserName, Version userVersion)
        {
            return browserName.Equals(Vendor, StringComparison.OrdinalIgnoreCase)
                && MatchesVersion(userVersion);
        }

        protected abstract bool MatchesVersion(Version userVersion);
    }

    public class LessThanOrEqualRule : BrowserRule
    {
        private readonly Version _maxVersion;

        public LessThanOrEqualRule(string vendor, Version maxVersion)
            : base(vendor)
        {
            _maxVersion = maxVersion;
        }

        protected override bool MatchesVersion(Version userVersion) => userVersion <= _maxVersion;
    }

    public class RangeRule : BrowserRule
    {
        private readonly int _majorVersion;

        public RangeRule(string vendor, int majorVersion)
            : base(vendor)
        {
            _majorVersion = majorVersion;
        }

        protected override bool MatchesVersion(Version userVersion) =>
            userVersion.Major == _majorVersion;
    }

    public class ExactMatchRule : BrowserRule
    {
        private readonly Version _exactVersion;

        public ExactMatchRule(string vendor, Version exactVersion)
            : base(vendor)
        {
            _exactVersion = exactVersion;
        }

        protected override bool MatchesVersion(Version userVersion) => userVersion == _exactVersion;
    }
}
