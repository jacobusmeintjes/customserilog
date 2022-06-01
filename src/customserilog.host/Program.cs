using customserilog.enrichers;
using customserilog.enrichers.TraceIdentifier;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.UseSerilog((b, loggerConfiguration) =>
{
    loggerConfiguration
        .Enrich.With<AspnetcoreHttpcontextEnricher>()
        .Enrich.With<TraceIdentifierEnricher>()
        .WriteTo.Console()
        .WriteTo.Seq("http://localhost:5341");
});
// builder.Logging.ClearProviders();
// var logger = new LoggerConfiguration()
//     .WriteTo.Console()
//     .WriteTo.Seq("http://localhost:5341")
//     .CreateLogger();


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