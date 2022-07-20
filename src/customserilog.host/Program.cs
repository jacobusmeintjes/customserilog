using customserilog.enrichers;
using customserilog.enrichers.Middleware;
using customserilog.enrichers.TraceIdentifier;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.UseSerilog((b, loggerConfiguration) =>
{
    loggerConfiguration
        .MinimumLevel.Information()
        //.Enrich.With<AspnetcoreHttpcontextEnricher>()
        .Enrich.With<TraceIdentifierEnricher>()
        //.Enrich.With<HeaderItemEnricher>()
        //.Enrich.With<QueryParameterEnricher>()
        .WriteTo.Console()
        .WriteTo.Seq("http://localhost:5341");
});
// builder.Logging.ClearProviders();
// var logger = new LoggerConfiguration()
//     .WriteTo.Console()
//     .WriteTo.Seq("http://localhost:5341")
//     .CreateLogger();

// builder.Services.AddHttpLogging(options =>
// {
//     options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders | HttpLoggingFields.ResponseBody;
//     //options.LoggingFields = HttpLoggingFields.All;
//     options.RequestHeaders.Add("Username");
//     options.RequestHeaders.Add("version");
// });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<RequestLoggingMiddleware>();

//app.UseHttpLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


 static MyObject CustomEnricherLogic(IHttpContextAccessor ctx)
{
    var context = ctx.HttpContext;
    if (context == null) return null;
            
    var myInfo = new MyObject
    {
        Path = context.Request.Path.ToString(),
        Host = context.Request.Host.ToString(),
        Method = context.Request.Method
    };

    var user = context.User;
    if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
    {
        myInfo.UserClaims = user.Claims.Select(a => new KeyValuePair<string, string>(a.Type, a.Value)).ToList();
    }
    return myInfo;
}

public class MyObject
{
    public string Path { get; set; }
    public string Host { get; set; }
    public string Method { get; set; }

    public List<KeyValuePair<string, string>> UserClaims { get; set; }
}