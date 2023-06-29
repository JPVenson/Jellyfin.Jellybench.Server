using Jellyfin.Jellybench.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jellyfin.Jellybench.Database;

public class JellybenchDataContext : DbContext
{
    public JellybenchDataContext(DbContextOptions options) : base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Jellybench");
        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<BenchRun> BenchRuns { get; set; }
    public DbSet<DataPoint> DataPoints { get; set; }
    public DbSet<RequestDataPoint> RequestDataPoints { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BenchRun>()
            .HasKey(e => e.BenchRunId);
        modelBuilder.Entity<BenchRun>()
            .HasIndex(e => new { e.DataRequestKey });
        modelBuilder.Entity<BenchRun>()
            .HasIndex(e => new { e.GpuName, e.GpuManufacture });
        modelBuilder.Entity<BenchRun>()
            .HasIndex(e => new { e.CpuName, e.CpuManufacture });
        modelBuilder.Entity<BenchRun>()
            .HasIndex(e => new { e.CpuName, e.SystemRam });
        modelBuilder.Entity<BenchRun>()
            .HasIndex(e => new { e.GpuName, e.GpuRam });

        modelBuilder.Entity<BenchRun>()
            .HasMany(e => e.DataPoints)
            .WithOne(e => e.BenchRun)
            .HasForeignKey(e => e.IdBenchRun)
            .IsRequired();

        modelBuilder.Entity<DataPoint>()
            .HasKey(e => e.DataPointId);
        modelBuilder.Entity<DataPoint>()
            .HasOne(e => e.BenchRun)
            .WithMany(e => e.DataPoints);
        modelBuilder.Entity<DataPoint>()
            .HasOne(e => e.RequestDataPoint)
            .WithMany(e => e.DataPoints);

        modelBuilder.Entity<RequestDataPoint>()
            .HasKey(e => e.RequestDataPointId);

    }
}