using Angular.App.Services.Contracts;

namespace Angular.App.Services.Extensions
{
    public static class BrowserCheckServiceExtension
    {
        public static IServiceCollection AddBrowserCheck(this IServiceCollection services)
        {
            return services.AddSingleton<IBrowserCheckService, BrowserCheckService>();
        }
    }
}
