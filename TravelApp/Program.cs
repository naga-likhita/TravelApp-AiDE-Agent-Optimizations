using Microsoft.EntityFrameworkCore;
using TravelApp;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext first
builder.Services.AddDbContextPool<TravelDbContext>(options =>
    options.UseSqlite(TravelDbContext.DbPath),
    poolSize: 128);

// Add caching services
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialize database using DI
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TravelDbContext>();
    context.Import();
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
