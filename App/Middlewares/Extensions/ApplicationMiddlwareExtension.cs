namespace Angular.App.Middlewares.Extensions
{
    public static class ApplicationMiddlewareExtension
    {
        public static void UseApplicationMiddlewares(
            this WebApplication app,
            IWebHostEnvironment env
        )
        {
            //? Environment-specific middlewares
            if (env.IsDevelopment())
            {
                //? Swagger engine and UI add
                app.UseSwagger(); //* Enable Swagger endpoint
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Error"); //* Custom error page for unhandled exceptions
                app.UseHsts(); //* Enforce HTTP Strict Transport Security
            }

            //? Built-in middleware
            app.UseStaticFiles();
            app.UseHttpsRedirection(); //* Redirect all HTTP requests to HTTPS
            app.UseRouting();
            app.MapControllers();

            //? Custom middleware setup (your version 3.0 logic)
            app.UseIPBrowserCheck(env); //* Handle the ordered middleware registration
        }
    }
}
