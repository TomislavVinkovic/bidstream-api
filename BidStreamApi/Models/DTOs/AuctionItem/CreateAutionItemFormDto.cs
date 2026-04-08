namespace BidStream.Models.DTOs.AuctionItem;

public class CreateAuctionItemFormDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required decimal StartingPrice { get; set; }
    public required DateTime EndTime { get; set; }
    public required IFormFileCollection Images {get; set;}
}