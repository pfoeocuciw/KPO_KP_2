using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration config)
        {
            services.AddDbContext<FileStorageDbContext>(opts =>
                opts.UseNpgsql(config.GetConnectionString("DefaultConnection")));
            services.AddScoped<IFileRepository, FileRepository>();
            return services;
        }
    }
}