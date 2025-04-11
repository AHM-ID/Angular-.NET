namespace Angular.App.Services.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddControllers(); //? Registers MVC controllers

            //* Register custom services after
            services
                .AddSwaggerGen() //? Registers Swagger for Development Environment
                .AddInvalidBrowserCheck(configuration) //? Reads invalid browser list from config
                .AddIPValidation() //? Registers IP validation services
                .AddBrowserCheck(); //? Registers browser check services

            return services;
        }
    }
}
