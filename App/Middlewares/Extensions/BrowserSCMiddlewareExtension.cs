namespace Angular.App.Middlewares.Extensions
{
    public static class BrowserSCMiddlewareExtension
    {
        public static IApplicationBuilder UseBrowserSC(this IApplicationBuilder app)
        {
            //? Pre Condition
            return app.UseMiddleware<BrowserSCMiddleware>();
        }
    }
}
