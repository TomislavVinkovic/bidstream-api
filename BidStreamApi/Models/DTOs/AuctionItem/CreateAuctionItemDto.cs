using BidStream.Models.DTOs.AuctionImage;

namespace BidStream.Models.DTOs.AuctionItem;

public class CreateAuctionItemDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required decimal StartingPrice { get; set; }
    public required DateTime EndTime { get; set; }
    public required List<CreateAuctionImageDto> Images { get; set; } = new List<CreateAuctionImageDto>();
}