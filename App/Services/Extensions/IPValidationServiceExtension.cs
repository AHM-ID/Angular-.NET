using Angular.App.Services.Contracts;

namespace Angular.App.Services.Extensions
{
    public static class IPValidationServiceExtension
    {
        public static IServiceCollection AddIPValidation(this IServiceCollection services)
        {
            return services.AddSingleton<IIPValidationService, IPValidationService>();
        }
    }
}
