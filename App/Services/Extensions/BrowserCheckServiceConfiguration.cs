using Angular.App.Services.Options;

namespace Angular.App.Services.Extensions
{
    public static class BrowserCheckServiceConfiguration
    {
        public static IServiceCollection AddInvalidBrowserCheck(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            //! Bind the InvalidBrowsers section from appsettings to strongly typed options
            services.Configure<BrowserCheckServiceOptions>(
                configuration.GetSection("InvalidBrowsers")
            );

            return services;
        }
    }
}
