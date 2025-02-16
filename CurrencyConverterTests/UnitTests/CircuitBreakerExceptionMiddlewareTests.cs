using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Moq;
using Polly.CircuitBreaker;
using Xunit;

namespace CurrencyConverterTests.UnitTests
{
    public class CircuitBreakerExceptionMiddlewareTests
    {
        private readonly Mock<ILogger<CircuitBreakerExceptionMiddleware>> _mockLogger;

        public CircuitBreakerExceptionMiddlewareTests()
        {
            _mockLogger = new Mock<ILogger<CircuitBreakerExceptionMiddleware>>();
        }

       

        [Fact]
        public async Task Middleware_Should_Allow_Request_When_No_Exception()
        {
            // Arrange: Middleware should allow normal requests if no exception occurs
            var testServer = new TestServer(new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseMiddleware<CircuitBreakerExceptionMiddleware>();
                    app.Run(async context =>
                    {
                        await context.Response.WriteAsync("service running");
                    });
                })
            );

            var client = testServer.CreateClient();

            // Act: Send a normal request
            var response = await client.GetAsync("/");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Be("service running");
        }

        [Fact]
        public async Task Middleware_Should_Log_Error_On_BrokenCircuitException()
        {
            // Arrange: Mock RequestDelegate to throw BrokenCircuitException
            var mockNext = new Mock<RequestDelegate>();
            mockNext.Setup(n => n(It.IsAny<HttpContext>()))
                    .ThrowsAsync(new BrokenCircuitException("Circuit is open due to failures."));

            var context = new DefaultHttpContext();
            var middleware = new CircuitBreakerExceptionMiddleware(mockNext.Object, _mockLogger.Object);

            // Act
            await middleware.InvokeAsync(context);

            // Assert: Logger should be called with the expected message
            _mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((obj, type) => obj.ToString().Contains("Circuit Breaker Opened")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        private class ErrorResponse
        {
            public string Message { get; set; } = string.Empty;
            public string Details { get; set; } = string.Empty;
        }
    }
}
