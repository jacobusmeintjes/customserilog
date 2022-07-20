using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace customserilog.enrichers.TraceIdentifier;

public class QueryParameterEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _contextAccessor;
    private const string QueryPrefix = "query";

    public QueryParameterEnricher() : this(new HttpContextAccessor())
    {
    }

    public QueryParameterEnricher(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (_contextAccessor.HttpContext == null) return;

        foreach (var param in _contextAccessor.HttpContext.Request.Query)
        {
            var property = propertyFactory.CreateProperty($"{QueryPrefix}.{param.Key}",
                param.Value.ToString());
            logEvent.AddOrUpdateProperty(property);
        }
    }
}