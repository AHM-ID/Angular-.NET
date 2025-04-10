using System.Net;

namespace Angular.App.Services.Contracts
{
    public interface IIPValidationService
    {
        Task<bool> ValidateIP(IPAddress remoteIp);
    }
}
