using BidStream.Common;
using BidStream.Data;
using BidStream.Models.DTOs.AuctionItem;
using BidStream.Models.Entities;
using BidStream.Services.Interface;
using Mapster;

namespace BidStream.Services;

public class AuctionItemService : IAuctionItemService
{

    private readonly AppDbContext _context;
    private readonly IFileService _fileService;

    public AuctionItemService(
        AppDbContext context,
        IFileService fileService
    )
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<ServiceResult<AuctionItemListResponse>> ListAsync(AuctionItemQueryParameters query)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResult<AuctionItemResponse>> CreateAsync(CreateAuctionItemDto dto, Guid userId)
    {
        var auctionItem = dto.Adapt<AuctionItem>();
        auctionItem.SellerId = userId;

        _context.AuctionItems.Add(auctionItem);
        await _context.SaveChangesAsync();

        await _context.Entry(auctionItem)
            .Reference(a => a.Seller)
            .LoadAsync();
        
        var auctionItemDto = AuctionItemDtoFactory(auctionItem);
        var response = new AuctionItemResponse(auctionItemDto);

        return ServiceResult<AuctionItemResponse>.Ok(response);
    }

    private AuctionItemDto AuctionItemDtoFactory(AuctionItem auctionItem)
    {
        var dto = auctionItem.Adapt<AuctionItemDto>();
        var absoluteImageUrls = new List<string>();

        foreach(var image in auctionItem.Images)
        {
            absoluteImageUrls.Add(_fileService.GetAbsoluteFileUrl(image.ImageUrl)!);
        }
        dto.Images = absoluteImageUrls;

        return dto;
    }
}