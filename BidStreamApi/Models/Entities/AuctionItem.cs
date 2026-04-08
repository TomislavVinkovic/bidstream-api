using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BidStream.Models.Entities;

[PrimaryKey("Id")]
public class AuctionItem : BaseEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal StartingPrice { get; set; }
    public decimal CurrentHighestBid { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsClosed { get; set; } = false;
    
    public Guid SellerId { get; set; }
    public User Seller { get; set; } = null!;
    
    public Guid? WinnerId { get; set; }
    public User? Winner { get; set; }

    // EF Core Concurrency Token
    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public ICollection<Bid> Bids { get; set; } = new List<Bid>();
    public ICollection<AuctionImage> Images { get; set; } = new List<AuctionImage>();
}