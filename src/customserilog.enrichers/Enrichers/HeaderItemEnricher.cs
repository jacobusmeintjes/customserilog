using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace customserilog.enrichers.TraceIdentifier;

public class HeaderItemEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _contextAccessor;
    private const string HeaderPrefix = "header";

    public HeaderItemEnricher() : this(new HttpContextAccessor())
    {
    }

    public HeaderItemEnricher(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (_contextAccessor.HttpContext == null) return;
        
        foreach (var header in _contextAccessor.HttpContext.Request.Headers)
        {
            var property = propertyFactory.CreateProperty($"{HeaderPrefix}.{header.Key}",
                header.Value.ToString());
            logEvent.AddOrUpdateProperty(property);
        }
    }
}