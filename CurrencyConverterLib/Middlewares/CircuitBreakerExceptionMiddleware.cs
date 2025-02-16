using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
public class CircuitBreakerExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CircuitBreakerExceptionMiddleware> _logger;
    public CircuitBreakerExceptionMiddleware(RequestDelegate next, ILogger<CircuitBreakerExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Call the next middleware in the pipeline
            await _next(context);
        }
        catch (BrokenCircuitException ex)
        {
            // Log the exception for monitoring purposes
            _logger.LogError(ex, "Circuit Breaker Opened: Service temporarily unavailable");

            // Handle the BrokenCircuitException and send a custom response
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            context.Response.ContentType = "application/json";

            // Return a detailed error message to the client
            var errorResponse = new
            {
                message = "The service is temporarily unavailable due to high failure rates. Please try again later.",
                details = ex.Message
            };

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}
