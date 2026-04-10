namespace BidStream.Models.DTOs.AuctionItem;

public class UpdateAuctionItemResult
{
    public AuctionItemResponse Response { get; set; } = null!;
    public List<string> DeletedImageUrls { get; set; } = new();
}