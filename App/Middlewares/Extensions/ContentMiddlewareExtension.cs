namespace Angular.App.Middlewares.Extensions
{
    public static class ContentMiddlewareExtension
    {
        public static IApplicationBuilder UseContent(this IApplicationBuilder app)
        {
            //? Pre Condition
            return app.UseMiddleware<ContentMiddleware>();
        }
    }
}
