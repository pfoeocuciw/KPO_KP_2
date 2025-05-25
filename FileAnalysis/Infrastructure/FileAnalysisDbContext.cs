using Microsoft.EntityFrameworkCore;

namespace Infrastructure;
public class FileAnalysisDbContext : DbContext
{
    public DbSet<AnalysisEntity> Analyses { get; set; } = null!;

    public FileAnalysisDbContext(DbContextOptions<FileAnalysisDbContext> opts)
        : base(opts) { }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<AnalysisEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.FileId).IsUnique();
            e.Property(x => x.CreatedUtc).IsRequired();
        });
    }
}