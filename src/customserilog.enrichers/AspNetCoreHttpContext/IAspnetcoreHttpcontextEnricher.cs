using Microsoft.AspNetCore.Http;
using Serilog.Core;

namespace customserilog.enrichers;

public interface IAspnetcoreHttpcontextEnricher : ILogEventEnricher
{
    void SetCustomAction(Func<IHttpContextAccessor, object> customAction);
}