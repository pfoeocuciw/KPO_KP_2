using Microsoft.OpenApi.Models;
using Infrastructure;
using Application;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("storage", c =>
    c.BaseAddress = new Uri(builder.Configuration["StorageServiceUrl"]!));
builder.Services.AddHttpClient("analysisStorage", c =>
    c.BaseAddress = new Uri(builder.Configuration["StorageServiceUrl"]!));
builder.Services.AddHttpClient("wordcloudapi", c =>
    c.BaseAddress = new Uri(builder.Configuration["WordCloudApiUrl"]!));

var cfg = builder.Configuration;

builder.Services.AddInfrastructure(cfg);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(AnalyzeFileCommand).Assembly));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "File Analysis API", Version = "v1" });
});

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FileAnalysisDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();