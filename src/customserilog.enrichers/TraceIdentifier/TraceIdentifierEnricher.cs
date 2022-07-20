using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace customserilog.enrichers.TraceIdentifier;

public class TraceIdentifierEnricher
    : ILogEventEnricher
{
    private const string TraceIdentifierPropertyName = "TraceIdentifier";
    private readonly IHttpContextAccessor _contextAccessor;

    public TraceIdentifierEnricher() : this(new HttpContextAccessor())
    {
    }

    public TraceIdentifierEnricher(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }


    /// <summary>
    /// Enrich the log event.
    /// </summary>
    /// <param name="logEvent">The log event to enrich.</param>
    /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (_contextAccessor.HttpContext is not null)
        {
            var property = propertyFactory.CreateProperty(TraceIdentifierPropertyName,
                _contextAccessor.HttpContext.TraceIdentifier);
            logEvent.AddOrUpdateProperty(property);

            foreach (var param in _contextAccessor.HttpContext.Request.Query)
            {
                property = propertyFactory.CreateProperty($"query.{param.Key}",
                    param.Value.ToString());
                logEvent.AddOrUpdateProperty(property);
            }

            foreach (var header in _contextAccessor.HttpContext.Request.Headers)
            {
                property = propertyFactory.CreateProperty($"header.{header.Key}",
                    header.Value.ToString());
                logEvent.AddOrUpdateProperty(property);
            }
        }
    }
}