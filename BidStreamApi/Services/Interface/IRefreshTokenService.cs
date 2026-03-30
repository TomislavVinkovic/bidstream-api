namespace BidStream.Services.Interface;

using BidStream.Models.DTOs.RefreshToken;
using BidStream.Models.Entities;

public interface IRefreshTokenService
{
    public Task<(string refreshToken, string hashedRefreshToken)> CreateAsync(CreateRefreshTokenDto dto);
    public Task<RefreshToken?> GetByTokenAsync(string rawToken);
    public Task<RefreshToken?> GetByTokenAndUserAsync(string rawToken, Guid userId);
    public Task<bool> RevokeAsync(string rawToken);
    public Task<bool> RevokeFamilyAsync(Guid tokenFamily);
    public Task<(string refreshToken, string hashedRefreshToken)?> RenewTokenAsync(RenewRefreshTokenDto dto);
}