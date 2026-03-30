using BidStream.Models;
using BidStream.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BidStream.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users { get; set; }
    public DbSet<User> Bids { get; set; }
    public DbSet<AuctionImage> AuctionImages { get; set; }
    public DbSet<AuctionItem> AuctionItems { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = DateTime.UtcNow;

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships

        modelBuilder.Entity<Bid>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bids)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Bid>()
            .HasOne(b => b.AuctionItem)
            .WithMany(a => a.Bids)
            .HasForeignKey(b => b.AuctionItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Row version rule, critical for concurency
        modelBuilder.Entity<AuctionItem>()
            .Property(a => a.RowVersion)
            .IsRowVersion();

        modelBuilder.Entity<AuctionImage>()
            .HasOne(i => i.AuctionItem)
            .WithMany(a => a.Images)
            .HasForeignKey(i => i.AuctionItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
