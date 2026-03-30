namespace BidStream.Models.Entities;

public class Bid : BaseEntity
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
    
    public Guid AuctionItemId { get; set; }
    public AuctionItem AuctionItem { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}