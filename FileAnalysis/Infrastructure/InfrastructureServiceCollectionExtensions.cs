using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Domain;
using Application;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;
public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<FileAnalysisDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAnalysisRepository, SqlAnalysisRepository>();

        services.AddSingleton<ITextAnalyzer, TextAnalyzer>();

        services.AddHttpClient("storage", c =>
            c.BaseAddress = new Uri(config["StorageServiceUrl"]!));
        services.AddHttpClient("wordcloudapi", c =>
        {
            c.BaseAddress = new Uri(config["WordCloudApiUrl"]!); 
        });

        return services;
    }
}