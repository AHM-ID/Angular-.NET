using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Angular.App.Options;
using Angular.App.Services.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Angular.App.Services
{
    public class IPValidationService : IIPValidationService
    {
        private readonly string[] _allowedIPs;
        private readonly ILogger<IPValidationService> _logger;

        public IPValidationService(
            IConfiguration configuration,
            ILogger<IPValidationService> logger
        )
        {
            _allowedIPs =
                configuration.GetSection("AllowedIPs").Get<string[]>() ?? new string[] { };
            _logger = logger;
        }

        public async Task<bool> ValidateIP(IPAddress remoteIp)
        {
            return await Task.Run(() =>
            {
                //? Normalize the IP: convert IPv6 loopback (::1) to IPv4 (127.0.0.1)
                if (remoteIp != null)
                {
                    if (remoteIp.Equals(IPAddress.IPv6Loopback))
                    {
                        remoteIp = IPAddress.Loopback; //* 127.0.0.1
                    }
                    else if (remoteIp.IsIPv4MappedToIPv6)
                    {
                        remoteIp = remoteIp.MapToIPv4();
                    }
                }

                //? Use fallback IP if remoteIp is null
                string remoteIpString = remoteIp?.ToString() ?? "127.0.0.0";

                //? Log the IP being checked
                _logger.LogInformation("Checking IP: {RemoteIP}", remoteIpString);

                //? Check if the IP is in the allowed list
                bool isAllowed = _allowedIPs.Contains(remoteIpString);

                if (!isAllowed)
                {
                    //? Log blocked access
                    _logger.LogWarning(
                        "IP validation failed: {RemoteIP} is not in the allowed list.",
                        remoteIpString
                    );
                }
                else
                {
                    //? Log allowed access
                    _logger.LogInformation(
                        "IP validation succeeded: {RemoteIP} is allowed.",
                        remoteIpString
                    );
                }

                return isAllowed;
            });
        }
    }
}
