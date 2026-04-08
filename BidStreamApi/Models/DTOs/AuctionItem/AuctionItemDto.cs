using BidStream.Models.DTOs.Profile;

namespace BidStream.Models.DTOs.AuctionItem;

public class AuctionItemDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required decimal CurrentHighestBid { get; set; }
    public required bool IsClosed { get; set; }
    public required DateTime EndTime { get; set; }

    public required ProfileDto Seller { get; set; }
    public ProfileDto? Winner { get; set; }
    public required List<string> Images { get; set; }
}