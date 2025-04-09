using Microsoft.Extensions.Primitives;

namespace Angular.App.Services.Contracts
{
    public interface IBrowserCheckService
    {
        Task<bool> ValidateBrowser(StringValues header);
    }
}
