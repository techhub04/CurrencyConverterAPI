using CurrencyConverterLib.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace CurrencyConverterTests.UnitTests
{
    public class RequestResponseLoggingMiddlewareTests
    {
        private readonly Mock<ILogger<RequestResponseLoggingMiddleware>> _mockLogger;
        private readonly RequestResponseLoggingMiddleware _middleware;

        public RequestResponseLoggingMiddlewareTests()
        {
            _mockLogger = new Mock<ILogger<RequestResponseLoggingMiddleware>>();
            _middleware = new RequestResponseLoggingMiddleware(async (innerHttpContext) =>
            {
                innerHttpContext.Response.StatusCode = 200;
                await innerHttpContext.Response.WriteAsync("Test Response");
            }, _mockLogger.Object);
        }

        [Fact]
        public async Task Invoke_LogsRequestAndResponse()
        {
            var context = new DefaultHttpContext();
            context.Request.Method = "GET";
            context.Request.Path = "/test";
            context.Request.QueryString = new QueryString("?query=1");
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("Test Body"));
            context.Request.ContentLength = "Test Body".Length;
            context.Response.Body = new MemoryStream();

            await _middleware.Invoke(context);

            _mockLogger.Verify(log => log.Log(
                It.Is<LogLevel>(level => level == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Incoming Request")),
                It.IsAny<System.Exception>(),
                It.IsAny<Func<It.IsAnyType, System.Exception, string>>()
            ), Times.Once);

            // Fluent Assertions for verifying the logs
            _mockLogger.Invocations.Count.Should().Be(4);
            _mockLogger.Invocations[2].Arguments[2].ToString().Should().Contain("Incoming Request");
        }

        [Fact]
        public async Task Invoke_LogsClientIp()
        {
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
            context.Response.Body = new MemoryStream();

            await _middleware.Invoke(context);

            _mockLogger.Verify(log => log.Log(
                It.Is<LogLevel>(level => level == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Client Id")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, System.Exception, string>>()
            ), Times.Once);

            // Fluent Assertions for verifying the logs
            _mockLogger.Invocations.Count.Should().Be(4);
            _mockLogger.Invocations[0].Arguments[2].ToString().Should().Contain("Client Id");
        }

        [Fact]
        public async Task Invoke_LogsResponseStatusCode()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await _middleware.Invoke(context);

            _mockLogger.Verify(log => log.Log(
                It.Is<LogLevel>(level => level == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Outgoing Response: Status: 200")),
                It.IsAny<System.Exception>(),
                It.IsAny<Func<It.IsAnyType, System.Exception, string>>()
            ), Times.Once);

            // Fluent Assertions for verifying the logs
            _mockLogger.Invocations.Count.Should().Be(4);
            _mockLogger.Invocations[3].Arguments[2].ToString().Should().Contain("Outgoing Response: Status: 200");
        }
    }
}
