namespace BidStream.Models.DTOs.AuctionItem;

public class UpdateAuctionItemDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? StartingPrice { get; set; }
    public DateTime? EndTime { get; set; }
    
    public List<string> NewImageUrls { get; set; } = new();
    public List<Guid> ImagesToRemove { get; set; } = new();
}