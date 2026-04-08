namespace BidStream.Models.DTOs.AuctionItem;

public class AuctionItemListResponse
{
    public required List<AuctionItemDto> AuctionItems { get; set; }
    public required int AuctionItemsCount { get; set; }
}