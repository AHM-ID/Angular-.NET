namespace Angular.App.Middlewares.Extensions
{
    public static class ErrorHandlingMiddlewareExtension
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
        {
            //? Pre Condition
            return app.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
