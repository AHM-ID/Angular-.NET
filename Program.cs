using Angular.App.Middlewares.Extensions;
using Angular.App.Services.Extensions;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);


#region Builder Configuration
//? Load configuration from appsettings.json with hot-reload enabled for dynamic updates
{
    builder
        .Configuration.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile(
            $"appsettings.{builder.Environment.EnvironmentName}.json",
            optional: true,
            reloadOnChange: true
        )
        .AddEnvironmentVariables();
}


#endregion


#region Logging Configuration
//? Configure Serilog with a daily directory structure for organized log management
{
    //* Generate today's folder name (e.g., "2025-04-03") and determine base log path
    var dateFolder = DateTime.UtcNow.ToString("yyyy-MM-dd");
    var logBasePath = builder.Configuration.GetValue<string>("Logging:BasePath:Path") ?? "Logs";
    var dailyLogDir = Path.Combine(logBasePath, dateFolder);

    //* Ensure the daily log directory exists
    Directory.CreateDirectory(dailyLogDir);

    //* Define log file paths for different log levels
    var logPaths = new
    {
        Info = Path.Combine(dailyLogDir, "info.log"),
        Warning = Path.Combine(dailyLogDir, "warning.log"),
        Error = Path.Combine(dailyLogDir, "error.log"),
    };

    //? Initialize Serilog with optimized configuration
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration) //* Load settings from appsettings.json
        .WriteTo.Console() //* Enable console logging for immediate feedback
        .WriteTo.File(
            logPaths.Info,
            restrictedToMinimumLevel: LogEventLevel.Information,
            rollingInterval: RollingInterval.Infinite
        ) //* No rolling needed due to daily folders
        .WriteTo.File(
            logPaths.Warning,
            restrictedToMinimumLevel: LogEventLevel.Warning,
            rollingInterval: RollingInterval.Infinite
        )
        .WriteTo.File(
            logPaths.Error,
            restrictedToMinimumLevel: LogEventLevel.Error,
            rollingInterval: RollingInterval.Infinite
        )
        .CreateLogger();
}
#endregion

builder.Host.UseSerilog();

#region Web Host Configuration
//? Configure Kestrel server options from configuration
builder.WebHost.UseKestrel(options =>
{
    options.Configure(builder.Configuration.GetSection("Kestrel"));
});
#endregion

#region Service Registration & Configuration
// Register all application-specific services (IP validation, browser checks, etc.)
builder.Services.AddApplicationServices(builder.Configuration);
#endregion

var app = builder.Build();

#region Middleware Configuration
// Configure and apply all middlewares in the correct order based on environment
app.UseApplicationMiddlewares(app.Environment);
#endregion

app.Run();

#region Helper Methods

#region Version 1.0

// /// <summary>
// /// Configures the middleware pipeline with custom middleware for error handling and request validation.
// /// </summary>
// /// <param name="app">The WebApplication instance to configure.</param>
// /// <param name="env">The WebApplication environment instance.</param>
// static void ConfigureMiddleware(WebApplication app, IWebHostEnvironment env)
// {
//     switch (env.EnvironmentName)
//     {
//         case "Development":
//         {
//             //* Ordered middleware registration for request processing
//             app.UseMiddleware<ResponseEditingMiddleware>(); //* Handle responses
//             app.UseMiddleware<ContentMiddleware>(); //* Process content
//             break;
//         }
//         case "Staging":
//         case "Production":
//         {
//             //* Ordered middleware registration for request processing
//             app.UseMiddleware<ErrorHandlingMiddleware>(); //* Handle errors first
//             app.UseMiddleware<ResponseEditingMiddleware>(); //* Handle responses
//             app.UseMiddleware<IPValidationReqMiddleware>(); //* Validate request IP
//             app.UseMiddleware<BrowserReqMiddleware>(); //* Validate browser request
//             app.UseMiddleware<IPValidationSCMiddleware>(); //* IP validation Short Circuit
//             app.UseMiddleware<BrowserSCMiddleware>(); //* Browser validation Short Circuit
//             app.UseMiddleware<ContentMiddleware>(); //* Process content
//             break;
//         }
//     }
// }

#endregion

#region Version 2.0

// /// <summary>
// /// Configures the middleware pipeline with custom middleware for error handling and request validation.
// /// </summary>
// /// <param name="app">The WebApplication instance to configure.</param>
// /// <param name="env">The WebApplication environment instance.</param>
// static void ConfigureMiddleware(WebApplication app, IWebHostEnvironment env)
// {
//     switch (env.EnvironmentName)
//     {
//         case "Development":
//         {
//             //* Ordered middleware registration for request processing
//             app.UseResponseEdit(env); //* Handle responses
//             app.UseContent(env); //* Process content
//             break;
//         }
//         case "Staging":
//         case "Production":
//         {
//             //* Ordered middleware registration for request processing
//             app.UseErrorHandling(env); //* Handle errors first
//             app.UseResponseEdit(env); //* Handle responses
//             app.UseIPValidationReq(env); //* Validate request IP
//             app.UseBrowserReq(env); //* Validate browser request
//             app.UseIPValidationSC(env); //* IP validation Short Circuit
//             app.UseBrowserSC(env); //* Browser validation Short Circuit
//             app.UseContent(env); //* Process content
//             break;
//         }
//     }
// }

#endregion

#region Version 3.0

// /// <summary>
// /// Configures the middleware pipeline with custom middleware for error handling and request validation.
// /// </summary>
// /// <param name="app">The WebApplication instance to configure.</param>
// /// <param name="env">The WebApplication environment instance.</param>
// static void ConfigureMiddleware(WebApplication app, IWebHostEnvironment env)
// {
//     app.UseIPBrowserCheck(env); //* Handle the ordered middleware registration
// }

#endregion

#endregion
