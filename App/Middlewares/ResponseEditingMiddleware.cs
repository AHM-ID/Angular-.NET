using System.Net;

namespace Angular.App.Middlewares
{
    public class ResponseEditingMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseEditingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //? Pipeline (Request Editing)
            await _next(context);

            var error = context.Items["Error"] as string;

            if (string.IsNullOrEmpty(error))
            {
                var content = context.Items["Content"] as string;

                if (string.IsNullOrEmpty(content))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
                else
                {
                    //? Content Negotiation
                    //? Content Formatting (XML, JSON, Binary, ...) + (Compression) + (/*Encryption*/)

                    await context.Response.WriteAsync(content);
                }
            }
            else
            {
                await context.Response.WriteAsync(error);
            }
        }
    }
}
