namespace BidStream.Models.DTOs.AuctionItem;

public class AuctionItemQueryParameters
{
    public string? Title { get; set; }
    public bool? IsClosed { get; set; }
    public DateTime? EndTime { get; set; }
    public string? SellerName { get; set; }
    public decimal? BidLow { get; set; }
    public decimal? BidHigh { get; set; }
    public int Limit { get; set; } = 20;
    public int Offset { get; set; } = 0;
}