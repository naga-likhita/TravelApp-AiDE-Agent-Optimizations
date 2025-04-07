using Microsoft.EntityFrameworkCore;
using TravelApp;

TravelDbContext.Instance.Export();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TravelDbContext>(o =>
{
    o.UseSqlite(TravelDbContext.DbPath);
});

builder.Services.AddMemoryCache();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TravelRepo>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
