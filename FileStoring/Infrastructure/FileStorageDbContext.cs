using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class FileStorageDbContext : DbContext
    {
        public DbSet<FileEntity> Files { get; set; } = null!;

        public FileStorageDbContext(DbContextOptions<FileStorageDbContext> opts)
            : base(opts) { }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<FileEntity>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Hash).IsUnique();
                e.Property(x => x.Location).IsRequired();
            });
        }
    }
}