namespace Angular.App.Middlewares.Extensions
{
    public static class IPBrowserCheckMiddlewareExtension
    {
        //? Fluent Patten
        //? This pattern allow middlewares to be piped after each other
        public static IApplicationBuilder UseIPBrowserCheck(
            this IApplicationBuilder app,
            IWebHostEnvironment env
        )
        {
            //? Pre Process
            //? Environment Checking
            switch (env.EnvironmentName)
            {
                case "Development":
                {
                    //* Ordered middleware registration for request processing
                    //? Piping
                    app.UseResponseEdit() //* Handle responses
                        .UseContent(); //* Process content
                    break;
                }
                case "Staging":
                case "Production":
                {
                    //* Ordered middleware registration for request processing
                    //? Piping
                    app.UseErrorHandling() //* Handle errors first
                        .UseResponseEdit() //* Handle responses
                        .UseIPValidationReq() //* Validate request IP
                        .UseBrowserReq() //* Validate browser request
                        .UseIPValidationSC() //* IP validation Short Circuit
                        .UseBrowserSC() //* Browser validation Short Circuit
                        .UseContent(); //* Process content
                    break;
                }
            }

            return app;
        }
    }
}
