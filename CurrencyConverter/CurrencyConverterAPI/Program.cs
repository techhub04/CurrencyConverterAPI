using CurrencyConverterLib.Factory;
using CurrencyConverterLib.Factory.ConcreteProviders;
using CurrencyConverterLib.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddScoped<CurrencyProviderFactory>();
//builder.Services.AddScoped<ICurrencyProvider, FrankFurter>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
 


builder.Services.AddSwaggerGen();
builder.Services.AddControllers();



var app = builder.Build();

// Enable Swagger only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
