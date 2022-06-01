using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Configuration;

namespace customserilog.enrichers;

public static class LoggerEnrichmentConfigurationExtensions
{
    /// <summary>
    /// Enrich log events with Aspnetcore httpContext properties.
    /// </summary>
    /// <param name="enrichmentConfiguration">Logger enrichment configuration.</param>
    /// <param name="serviceProvider"></param>
    /// <returns>Configuration object allowing method chaining.</returns>
    public static LoggerConfiguration WithAspnetcoreHttpcontext(
        this LoggerEnrichmentConfiguration enrichmentConfiguration,
        IServiceProvider serviceProvider, Func<IHttpContextAccessor, object> customMethod = null)
    {
        if (enrichmentConfiguration == null) throw new ArgumentNullException(nameof(enrichmentConfiguration));

        var enricher = serviceProvider.GetService(typeof(AspnetcoreHttpcontextEnricher)) as IAspnetcoreHttpcontextEnricher;

        if (enricher is not null && customMethod != null)
            enricher.SetCustomAction(customMethod);

        return enrichmentConfiguration.With(enricher);
    }
}