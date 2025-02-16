using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Diagnostics;
using System.Text;

namespace CurrencyConverterLib.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var requestId = Activity.Current?.Id ?? context.TraceIdentifier;
           
            //
            var clientId = context.User.Claims.FirstOrDefault(c => c.Type == "ClientId")?.Value;
            _logger.LogInformation($"Client Id: {clientId}");

            // Get the client's IP address
            var clientIp = context.Connection.RemoteIpAddress?.ToString();

            // If behind a proxy, use X-Forwarded-For header
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                clientIp = context.Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim();
            }

            _logger.LogInformation($"Client IP: {clientIp}");


            using (LogContext.PushProperty("RequestId", requestId))
            {
                var request = await FormatRequest(context.Request);
                _logger.LogInformation("Incoming Request: {Request}", request);

                // Capture response
                var originalBodyStream = context.Response.Body;
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                var stopwatch = Stopwatch.StartNew();
                await _next(context);
                stopwatch.Stop();

                var response = await FormatResponse(context.Response);
                _logger.LogInformation("Outgoing Response: {Response} | Duration: {Duration}ms", response, stopwatch.ElapsedMilliseconds);

                // Copy response back to the original stream
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering();
            var body = await new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true).ReadToEndAsync();
            request.Body.Position = 0;
            return $"{request.Method} {request.Path} {request.QueryString} Body: {body}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return $"Status: {response.StatusCode} Body: {text}";
        }
    }
}
