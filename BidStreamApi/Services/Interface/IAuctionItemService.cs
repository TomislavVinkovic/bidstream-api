using BidStream.Common;
using BidStream.Models.DTOs.AuctionItem;

namespace BidStream.Services.Interface;

public interface IAuctionItemService
{
    public Task<ServiceResult<AuctionItemResponse>> GetByIdAsync(Guid id);
    public Task<ServiceResult<AuctionItemListResponse>> ListAsync(AuctionItemQueryParameters query);
    public Task<ServiceResult<AuctionItemResponse>> CreateAsync(CreateAuctionItemDto dto, Guid userId);
    public Task<ServiceResult<UpdateAuctionItemResult?>> UpdateAsync(Guid id, UpdateAuctionItemDto dto, Guid userId);
}