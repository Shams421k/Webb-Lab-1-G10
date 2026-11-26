using Microsoft.EntityFrameworkCore;
using Viskaexpressen.Models;

namespace Viskaexpressen.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Linje> Linjer => Set<Linje>();

    public DbSet<Tagsatt> Tagsatter => Set<Tagsatt>();

    public DbSet<Avgang> Avgangar => Set<Avgang>();

    public DbSet<Bokning> Bokningar => Set<Bokning>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Linje>()
            .Property(l => l.Pris)
            .HasPrecision(8, 2);

        modelBuilder.Entity<Tagsatt>()
            .HasIndex(t => t.Nummer)
            .IsUnique();

        modelBuilder.Entity<Avgang>()
            .HasOne(a => a.Linje)
            .WithMany(l => l.Avgangar)
            .HasForeignKey(a => a.LinjeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Avgang>()
            .HasOne(a => a.Tagsatt)
            .WithMany(t => t.Avgangar)
            .HasForeignKey(a => a.TagsattId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Bokning>()
            .HasOne(b => b.Avgang)
            .WithMany(a => a.Bokningar)
            .HasForeignKey(b => b.AvgangId)
            .OnDelete(DeleteBehavior.Cascade);

        // Samma plats får inte bokas två gånger på samma avgång.
        modelBuilder.Entity<Bokning>()
            .HasIndex(b => new { b.AvgangId, b.PlatsNummer })
            .IsUnique();

        modelBuilder.Entity<Bokning>()
            .HasIndex(b => b.Bokningsnummer)
            .IsUnique();
    }
}
