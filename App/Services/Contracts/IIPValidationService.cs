using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Angular.App.Services.Contracts
{
    public interface IIPValidationService
    {
        Task<bool> ValidateIP(IPAddress remoteIp);
    }
}
