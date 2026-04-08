using BidStream.Common;
using BidStream.Models.DTOs.AuctionItem;

namespace BidStream.Services.Interface;

public interface IAuctionItemService
{
    public Task<ServiceResult<AuctionItemResponse>> CreateAsync(CreateAuctionItemDto dto, Guid userId);
    public Task<ServiceResult<AuctionItemListResponse>> ListAsync(AuctionItemQueryParameters query);
}