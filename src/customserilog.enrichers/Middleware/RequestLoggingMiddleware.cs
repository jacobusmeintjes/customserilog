using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Core;


namespace customserilog.enrichers.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
    }

    public async Task Invoke(HttpContext context)
    {
        Stream originalBody = context.Response.Body;

        string responseBody = string.Empty;
        string requestBody = string.Empty;
        try
        {
            context.Request.EnableBuffering();
            using (var streamReader = new StreamReader(
                       context.Request.Body, System.Text.Encoding.UTF8, leaveOpen: true))
                requestBody = await streamReader.ReadToEndAsync();

            context.Request.Body.Position = 0;


            using (var memStream = new MemoryStream())
            {
                context.Response.Body = memStream;
                await _next(context);

                memStream.Position = 0;
                responseBody = await new StreamReader(memStream).ReadToEndAsync();

                memStream.Position = 0;
                await memStream.CopyToAsync(originalBody);
            }
        }
        finally
        {
            //var requestBody = new StreamReader(context.Request.Body).ReadToEnd();
            //var responseBody = new StreamReader(context.Response.Body).ReadToEnd();
            _logger.LogInformation(
                "Request {Method} {Url} => {StatusCode} {ContentType} {RequestBody} {ResponseBody}",
                context.Request?.Method,
                context.Request?.Path.Value,
                context.Response?.StatusCode,
                context.Response.ContentType, requestBody, responseBody);
        }
    }
}