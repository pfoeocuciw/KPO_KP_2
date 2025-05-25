using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("APIGateway.Tests")]
namespace Gateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddHttpClient("storage", c =>
        c.BaseAddress = new Uri(builder.Configuration["StorageServiceUrl"]!));
    builder.Services.AddHttpClient("analysis", c =>
        c.BaseAddress = new Uri(builder.Configuration["AnalysisServiceUrl"]!));
    builder.Services.AddHttpClient("wordcloudapi", c =>
        c.BaseAddress = new Uri(builder.Configuration["WordCloudApiUrl"]!));

    builder.Services
        .AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Gateway", Version = "v1" });
        c.OperationFilter<SwaggerFileUploadOperationFilter>(); 
    });

    var app = builder.Build();

    app.UseSwagger(c => c.RouteTemplate = "swagger/{documentName}/swagger.json");
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json",         "Gateway");
        c.RoutePrefix = "swagger";
    });

    app.MapPost("/upload",
            async ([FromForm] IFormFile file, IHttpClientFactory http) =>
            {
                var client = http.CreateClient("storage");
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);

                using var form = new MultipartFormDataContent();
                form.Add(new ByteArrayContent(ms.ToArray()), "file", file.FileName);

                var resp = await client.PostAsync("/api/files", form);
                resp.EnsureSuccessStatusCode();

                var json = await resp.Content.ReadAsStringAsync();
                return Results.Content(
                    json,
                    "application/json",
                    Encoding.UTF8,
                    StatusCodes.Status201Created
                );
            })
        .Accepts<IFormFile>("multipart/form-data")
        .Produces(StatusCodes.Status201Created)
        .DisableAntiforgery()
        .WithName("UploadFile")
        .WithOpenApi();

    app.MapGet("/files/{id:guid}", (Guid id, IHttpClientFactory f) =>
        f.CreateClient("storage").GetAsync($"/api/files/{id}")
            .ContinueWith(t => Results.Stream(t.Result.Content.ReadAsStream(), t.Result.Content.Headers.ContentType?.ToString())));

    app.MapGet("/files/{id:guid}/content", (Guid id, IHttpClientFactory f) =>
        f.CreateClient("storage").GetAsync($"/api/files/{id}/content")
            .ContinueWith(t => Results.Stream(t.Result.Content.ReadAsStream(), t.Result.Content.Headers.ContentType?.ToString())));

    app.MapPost("/analyze/{id:guid}", (Guid id, IHttpClientFactory f) =>
        f.CreateClient("analysis").PostAsync($"/api/analysis/{id}", null)
            .ContinueWith(t => Results.Stream(t.Result.Content.ReadAsStream(), t.Result.Content.Headers.ContentType?.ToString())));

    app.MapGet("/analyze/{id:guid}", async (Guid id, IHttpClientFactory f) =>
    {
        var client = f.CreateClient("analysis");
        var resp   = await client.GetAsync($"/api/analysis/{id}");

        if (resp.StatusCode == HttpStatusCode.NotFound)
            return Results.NotFound();

        if (resp.StatusCode == HttpStatusCode.NoContent)
            return Results.NoContent();

        resp.EnsureSuccessStatusCode();

        var body = await resp.Content.ReadAsStringAsync();
        return Results.Content(
            body,
            resp.Content.Headers.ContentType?.ToString() ?? "application/json"
        );
    });
    app.MapGet("/wordcloud/{id:guid}", (Guid id, IHttpClientFactory f) =>
        f.CreateClient("analysis").GetAsync($"/api/analysis/wordcloud/{id}")
            .ContinueWith(t => Results.Stream(t.Result.Content.ReadAsStream(), "image/png")));

    app.MapGet("/compare/{a:guid}/{b:guid}", (Guid a, Guid b, IHttpClientFactory f) =>
        f.CreateClient("analysis").GetAsync($"/api/analysis/similarity/{a}/{b}")
            .ContinueWith(t => Results.Stream(t.Result.Content.ReadAsStream(), t.Result.Content.Headers.ContentType?.ToString())));

    app.MapReverseProxy();

    app.Run();
    }
}
