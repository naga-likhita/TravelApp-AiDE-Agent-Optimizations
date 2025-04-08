using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Serilog.Enrichers.CorrelationId;
using Serilog.Formatting.Compact;
using TravelApp;

// Configure Serilog first
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()
    .WriteTo.Console(new CompactJsonFormatter())
    .CreateLogger();
TravelDbContext.Instance.MockDynamic();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddDbContextPool<TravelDbContext>(options =>
    options.UseSqlite(TravelDbContext.DbPath),
    poolSize: 128);

// Add caching services
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Add correlation middleware
app.UseMiddleware<CorrelationIdMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
