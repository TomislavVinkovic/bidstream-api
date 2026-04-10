using BidStream.Common;
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

    [HttpGet("{id}")]
    public async Task<ActionResult> GetArticle(Guid id)
    {
        var result = await _auctionItemService.GetByIdAsync(id);
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

    [HttpPost("{id}")]
    [Authorize]
    public async Task<ActionResult> Update(Guid id, UpdateAuctionItemRequest request)
    {
        var newImageUrls = new List<string>();
        if(request.auctionItem.NewImages != null && request.auctionItem.NewImages.Count > 0)
        {
            var filesToUpload = request.auctionItem.NewImages.Select(i => (i.OpenReadStream(), Path.GetExtension(i.FileName).ToLowerInvariant()));
            newImageUrls = await _fileService.UploadMultipleAsync(filesToUpload);
        }

        var updateDto = request.auctionItem.Adapt<UpdateAuctionItemDto>();
        updateDto.NewImageUrls = newImageUrls;

        var result = await _auctionItemService.UpdateAsync(id, updateDto, User.GetRequiredUserId());
        if(result.Success && result.Data != null)
        {
            foreach(var url in result.Data.DeletedImageUrls)
            {
                await _fileService.DeleteFile(url);
            }
            return Ok(result.Data.Response);
        }
        // Edge case: Update failed, delete the newly stored images
        else
        {
            foreach(var url in newImageUrls)
            {
                await _fileService.DeleteFile(url);
            }
            return HandleResult(ServiceResult<AuctionItemResponse>.BadRequest(result.Error!));
        }
    }
}