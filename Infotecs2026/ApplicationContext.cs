using CmdScale.EntityFrameworkCore.TimescaleDB;
using CmdScale.EntityFrameworkCore.TimescaleDB.Configuration.Hypertable;
using Infotecs2026.Share.Application.Interfaces;
using Infotecs2026.Share.Models;
using Microsoft.EntityFrameworkCore;

namespace Infotecs2026;

public class ApplicationContext : DbContext, IApplicationDbContext
{
    public DbSet<Value> Values { get; set; }
    public DbSet<Result> Results { get; set; }

    public ApplicationContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=usersdb;Username=postgres;Password=4423")
            .UseTimescaleDb();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Value>(entity =>
        {
            entity.ToTable("value");

            entity.Property(e => e.FileName)
            .IsRequired();

            entity.HasKey(e => new { e.Id, e.Date });

            entity.HasIndex(e => e.FileName);

            entity.HasIndex(e => e.Date);

            entity.IsHypertable(b => b.Date);
        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.ToTable("results");
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.FileName)
                  .IsUnique();

            entity.Property(e => e.FileName)
                  .IsRequired();

            entity.Property(e => e.ProcessedAt)
                  .HasDefaultValueSql("NOW()");
        });
    }
}