namespace Angular.App.Middlewares.Extensions
{
    public static class IPValidationSCMiddlewareExtension
    {
        public static IApplicationBuilder UseIPValidationSC(this IApplicationBuilder app)
        {
            //? Pre Condition
            return app.UseMiddleware<IPValidationSCMiddleware>();
        }
    }
}
