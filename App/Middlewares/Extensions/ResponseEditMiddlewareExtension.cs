namespace Angular.App.Middlewares.Extensions
{
    public static class ResponseEditMiddlewareExtension
    {
        public static IApplicationBuilder UseResponseEdit(this IApplicationBuilder app)
        {
            //? Pre Condition
            return app.UseMiddleware<ResponseEditingMiddleware>();
        }
    }
}
