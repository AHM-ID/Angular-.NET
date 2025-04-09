namespace Angular.App.Middlewares.Extensions
{
    public static class BrowserReqMiddlewareExtension
    {
        public static IApplicationBuilder UseBrowserReq(this IApplicationBuilder app)
        {
            //? Pre Condition
            return app.UseMiddleware<BrowserReqMiddleware>();
        }
    }
}
