using Application;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "FileStorage API",
        Version     = "v1",
        Description = "Хранение файлов в PostgreSQL"
    });
});
builder.Services.AddDbContext<FileStorageDbContext>(opts =>
    opts.UseNpgsql(connStr)
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Information));

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
var maxRetries = 10;
for (var i = 1; i <= maxRetries; i++)
{
    try
    {
        using(var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FileStorageDbContext>();
            db.Database.Migrate();
        }
        logger.LogInformation("Database is ready.");
        break;
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Database not ready yet (attempt {Attempt}/{Max}). Retrying in 2s...", i, maxRetries);
        await Task.Delay(2000);
    }
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileStorage API v1");
});

app.MapControllers();
app.Run();