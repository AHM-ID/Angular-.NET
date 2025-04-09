using Angular.App.Options;

namespace Angular.App.Services.Extensions
{
    public static class StaticVariablesServiceConfiguration
    {
        public static IServiceCollection AddStaticVariables(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.Configure<StaticVariablesOptions>(configuration.GetSection("StaticVariables"));

            return services;
        }
    }
}
