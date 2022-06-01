using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Hosting;

namespace customserilog.enrichers;

public static class WebhostBuilderExtensions
{
    public static WebApplicationBuilder UseSerilog(this WebApplicationBuilder builder, Action<WebApplicationBuilder, LoggerConfiguration> configureLogger)
    {
        builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        builder.Services.TryAddSingleton<IAspnetcoreHttpcontextEnricher, AspnetcoreHttpcontextEnricher>();

        builder.Logging.ClearProviders();
        var loggerConfiguration = new LoggerConfiguration();
            
        configureLogger(builder, loggerConfiguration);

        Logger logger = loggerConfiguration.CreateLogger();

        Log.Logger = logger;
        builder.Services.AddSingleton(services => (ILoggerFactory) new SerilogLoggerFactory(null, true));


        return builder;
    }
}