using System.Text;
using AspNetCoreRateLimit;
using CurrencyConverterLib.Extensions;
using CurrencyConverterLib.Factory;
using CurrencyConverterLib.Middlewares;
using CurrencyConverterLib.Services;
using CurrencyConverterLib.Services.Implementation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add HttpClient with Polly policies
builder.Services.AddHttpClient("ApiClient")
    .AddPolicyHandler(GetRetryPolicy())  // Retry policy
    .AddPolicyHandler(GetCircuitBreakerPolicy());  // Circuit breaker policy


// Retry policy: retry up to 3 times, waiting 2 seconds between retries
IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
        .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2));
}

// Circuit breaker policy: break after 3 consecutive failures for 10 seconds
IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
        .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10));  // Break after 3 failures within a rolling window of 10 seconds
}

builder.Services.AddScoped<CurrencyProviderFactory>();
 
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICacheService, CacheService>();

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Add memory cache for rate limiting
builder.Services.AddMemoryCache();

// Register rate limiting services
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

var configuration = builder.Configuration;
builder.AddLogger();

builder.Services.AddResponseCaching();


// JWT Authentication Configuration
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));
// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };
    });




builder.Services.AddOpenTelemetry().WithTracing(b => {
    b.SetResourceBuilder(
        ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
     .AddAspNetCoreInstrumentation()
     .AddOtlpExporter(opts => { opts.Endpoint = new Uri("http://localhost:4317"); });
});

builder.Services.AddAuthorization();
builder.Services.AddDistributedMemoryCache();

//API Versioning
builder.Services.AddEndpointsApiExplorer();

// Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true; // Include API version in response headers
    options.AssumeDefaultVersionWhenUnspecified = true; // Use default version if not specified
    options.DefaultApiVersion = new ApiVersion(1, 0); // Default version (v1.0)
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new QueryStringApiVersionReader("api-version")
    );
});

// Add API Explorer to support versioning in Swagger
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // Format: v1, v1.1, v2
    options.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

// Enable Swagger only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();


app.UseSerilogRequestLogging();

// Add Middleware
app.UseMiddleware<RequestResponseLoggingMiddleware>();
// Add the middleware to the pipeline
app.UseMiddleware<CircuitBreakerExceptionMiddleware>();
 

app.UseIpRateLimiting();
app.UseAuthorization();

app.MapControllers();

app.Run();

//For Integration Tests
public partial class Program { }