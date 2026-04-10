using BidStream.Common;
using BidStream.Data;
using BidStream.Models.DTOs.AuctionItem;
using BidStream.Models.Entities;
using BidStream.Services.Interface;
using Mapster;
using Microsoft.EntityFrameworkCore;

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

    public async Task<ServiceResult<AuctionItemResponse>> GetByIdAsync(Guid id)
    {
        var auctionItem = await _context.AuctionItems.FirstOrDefaultAsync(a => a.Id == id);
        if(auctionItem == null)
        {
            return ServiceResult<AuctionItemResponse>.NotFound($"Auction item with id {id} not found");
        }

        var dto = AuctionItemDtoFactory(auctionItem);
        var response = new AuctionItemResponse(dto);

        return ServiceResult<AuctionItemResponse>.Ok(response);
    }

    public async Task<ServiceResult<AuctionItemListResponse>> ListAsync(AuctionItemQueryParameters query)
    {
        var auctionItemsQuery = _context.AuctionItems
            .Include(a => a.Seller)
            .Include(a => a.Winner)
            .Include(a => a.Bids)
            .AsQueryable();
        
        // Querying
        if(!string.IsNullOrEmpty(query.Title))
        {
            auctionItemsQuery = auctionItemsQuery.Where(a => a.Title.ToLowerInvariant().Contains(query.Title.ToLowerInvariant()));
        }
        if(query.IsClosed != null)
        {
            auctionItemsQuery = auctionItemsQuery.Where(a => a.IsClosed == query.IsClosed);
        }
        if(query.EndTime != null)
        {
            // Fetch all auction items that have an end time before the designated end time
            auctionItemsQuery = auctionItemsQuery.Where(a => a.EndTime.CompareTo(query.EndTime) < 0);
        }
        if(query.BidLow != null)
        {
            auctionItemsQuery = auctionItemsQuery.Where(a => a.CurrentHighestBid >= query.BidLow);
        }
        if(query.BidHigh != null)
        {
            auctionItemsQuery = auctionItemsQuery.Where(a => a.CurrentHighestBid <= query.BidHigh);
        }

        // Sorting
        int sortDirection = 1;
        string sortBy = "createdat";

        List<int> validSortDirections = [-1, 1];
        List<string> validSortParameters = ["createdat", "currenthighestbid", "endtime", "title"];

        if(query.SortDirection.HasValue)
        {
            if(validSortDirections.Contains(query.SortDirection.Value))
            {
                sortDirection = query.SortDirection.Value;
            }
        }
        if(query.SortBy != null)
        {
            sortBy = query.SortBy.ToLowerInvariant();
        }

        if (sortDirection == 1)
        {
            auctionItemsQuery = sortBy switch
            {
                "title" => auctionItemsQuery.OrderBy(a => a.Title),
                "endtime" => auctionItemsQuery.OrderBy(a => a.EndTime),
                "currenthighestbid" => auctionItemsQuery.OrderBy(a => a.CurrentHighestBid),
                _ => auctionItemsQuery.OrderBy(a => a.CreatedAt)
            };
        }
        else
        {
            auctionItemsQuery = sortBy switch
            {
                "title" => auctionItemsQuery.OrderByDescending(a => a.Title),
                "endtime" => auctionItemsQuery.OrderByDescending(a => a.EndTime),
                "currenthighestbid" => auctionItemsQuery.OrderByDescending(a => a.CurrentHighestBid),
                _ => auctionItemsQuery.OrderByDescending(a => a.CreatedAt)
            };
        }

        var auctionItemsCount = await auctionItemsQuery.CountAsync();
        var auctionItems = auctionItemsQuery
            .Skip(query.Offset)
            .Take(query.Limit)
            .ToListAsync();
        
        var auctionItemDtos = auctionItems.Adapt<List<AuctionItemDto>>();
        var response = new AuctionItemListResponse
        {
            AuctionItems = auctionItemDtos,
            AuctionItemsCount = auctionItemsCount
        };

        return ServiceResult<AuctionItemListResponse>.Ok(response);
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

    public async Task<ServiceResult<UpdateAuctionItemResult?>> UpdateAsync(Guid id, UpdateAuctionItemDto dto, Guid userId)
    {
        var auction = await _context.AuctionItems
            .Include(a => a.Images)
            .FirstOrDefaultAsync(a => a.Id == id);
        
        if(auction == null)
        {
            return ServiceResult<UpdateAuctionItemResult?>.NotFound($"Auction with {id} not found.");
        }
        if (auction.SellerId != userId)
        {
            return ServiceResult<UpdateAuctionItemResult?>.Unauthorized();
        }

        if (!string.IsNullOrEmpty(dto.Title)) auction.Title = dto.Title;
        if (!string.IsNullOrEmpty(dto.Description)) auction.Description = dto.Description;
        if (dto.StartingPrice.HasValue) auction.StartingPrice = dto.StartingPrice.Value;
        if (dto.EndTime.HasValue) auction.EndTime = dto.EndTime.Value;

        var deletedUrls = new List<string>();

        if(dto.ImagesToRemove.Any())
        {
            var imagesToDelete = auction.Images.Where(i => dto.ImagesToRemove.Contains(i.Id));
            foreach(var image in imagesToDelete)
            {
                deletedUrls.Add(image.ImageUrl);
                auction.Images.Remove(image);
            }
        }

        foreach(var url in dto.NewImageUrls)
        {
            auction.Images.Add
            (
                new AuctionImage
                {
                    ImageUrl = url,
                    IsPrimary = false
                }
            );
        }

        // Ensure there is a primary image
        if (auction.Images.Any() && !auction.Images.Any(i => i.IsPrimary))
        {
            auction.Images.First().IsPrimary = true;
        }

        await _context.SaveChangesAsync();
        await _context.Entry(auction).Reference(a => a.Seller).LoadAsync();

        var auctionItemDto = AuctionItemDtoFactory(auction);
        var response = new AuctionItemResponse(auctionItemDto);

        var result = new UpdateAuctionItemResult
        {
            Response = response,
            DeletedImageUrls = deletedUrls
        };

        return ServiceResult<UpdateAuctionItemResult?>.Ok(result);
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