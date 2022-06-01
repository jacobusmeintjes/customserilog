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
        var property = propertyFactory.CreateProperty(TraceIdentifierPropertyName, _contextAccessor.HttpContext?.TraceIdentifier ?? "-");
        logEvent.AddOrUpdateProperty(property);
    }
}