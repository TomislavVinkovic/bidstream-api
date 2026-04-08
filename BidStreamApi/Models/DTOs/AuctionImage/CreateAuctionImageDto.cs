namespace BidStream.Models.DTOs.AuctionImage;

public class CreateAuctionImageDto
{
    public required string ImageUrl { get; set; }
    public required bool IsPrimary { get; set; }
}