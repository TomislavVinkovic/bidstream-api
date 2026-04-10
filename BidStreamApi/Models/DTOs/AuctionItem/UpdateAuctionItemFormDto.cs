namespace BidStream.Models.DTOs.AuctionItem;

public class UpdateAuctionItemFormDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? StartingPrice { get; set; } 
    public DateTime? EndTime { get; set; }

    public IFormFileCollection? NewImages { get; set; }

    public List<Guid>? ImagesToRemove { get; set; }
}