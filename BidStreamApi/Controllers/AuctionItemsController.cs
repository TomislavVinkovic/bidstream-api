using BidStream.Extensions;
using BidStream.Models.DTOs.AuctionImage;
using BidStream.Models.DTOs.AuctionItem;
using BidStream.Services.Interface;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BidStream.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuctionController : ApiControllerBase
{
    private readonly IAuctionItemService _auctionItemService;
    private readonly IFileService _fileService;
    public AuctionController(
        IAuctionItemService auctionItemService,
        IFileService fileService
    )
    {
        _auctionItemService = auctionItemService;
        _fileService = fileService;
    }

    [HttpGet("")]
    public async Task<ActionResult> List([FromQuery] AuctionItemQueryParameters query)
    {
        var result = await _auctionItemService.ListAsync(query);
        return HandleResult(result);
    }

    [HttpPost("")]
    [Authorize]
    public async Task<ActionResult> Create(CreateActionItemRequest request)
    {
        var filesToUpload = request.auctionItem.Images.Select(i => (i.OpenReadStream(), Path.GetExtension(i.FileName).ToLowerInvariant()));
        var imageUrls = await _fileService.UploadMultipleAsync(filesToUpload);

        var auctionItem = request.auctionItem.Adapt<CreateAuctionItemDto>();

        foreach(var url in imageUrls)
        {
            auctionItem.Images.Add
            (
                new CreateAuctionImageDto
                {
                    ImageUrl = url,
                    IsPrimary = auctionItem.Images.Count == 0 // Only the first image is primary
                }
            );
        }

        var result = await _auctionItemService.CreateAsync(auctionItem, User.GetRequiredUserId());
        return HandleResult(result);
    }
}