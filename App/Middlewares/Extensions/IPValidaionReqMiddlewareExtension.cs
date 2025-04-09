namespace Angular.App.Middlewares.Extensions
{
    public static class IPValidaionReqMiddlewareExtension
    {
        public static IApplicationBuilder UseIPValidationReq(this IApplicationBuilder app)
        {
            //? Pre Condition
            return app.UseMiddleware<IPValidationReqMiddleware>();
        }
    }
}
