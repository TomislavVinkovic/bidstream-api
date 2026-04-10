using BidStream.Models.DTOs.AuctionItem;
using BidStream.Models.DTOs.Profile;
using BidStream.Models.Entities;
using Mapster;

namespace BidStream.Mappers;

public static class MapsterConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<CreateAuctionItemFormDto, CreateAuctionItemDto>.NewConfig()
            .Ignore(dest => dest.Images);
           
        TypeAdapterConfig<CreateAuctionItemDto, AuctionItem>.NewConfig()
            .Map(dest => dest.CurrentHighestBid, src => src.StartingPrice)
            .Map(dest => dest.Images, src => src.Images.Select(i => i.Adapt<AuctionImage>()));

        TypeAdapterConfig<AuctionItem, AuctionItemDto>.NewConfig()
            .Map(dest => dest.Seller, src => src.Seller.Adapt<ProfileDto>())
            .Map(dest => dest.Winner, src => src.Winner.Adapt<ProfileDto>())
            .Ignore(dest => dest.Images);        
    }
}