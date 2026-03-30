using Microsoft.EntityFrameworkCore;

namespace BidStream.Models.Entities;

[PrimaryKey("Id")]
public class AuctionImage
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty; 
    
    // Identifies the main thumbnail image
    public bool IsPrimary { get; set; } 

    public Guid AuctionItemId { get; set; }
    public AuctionItem AuctionItem { get; set; } = null!;
}