using BidStream.Data;
using BidStream.Models.DTOs.RefreshToken;
using BidStream.Models.Entities;
using BidStream.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace BidStream.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;
    public RefreshTokenService(
        AppDbContext context,
        IJwtService jwtService
    )
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<(string refreshToken, string hashedRefreshToken)> CreateAsync(CreateRefreshTokenDto dto)
    {
        var (rawRefreshToken, hashedRefreshToken) = _jwtService.GenerateRefreshToken();

        _context.RefreshTokens.Add
        (
            new RefreshToken{
                UserId = dto.UserId,
                Token = hashedRefreshToken,
                Family =  dto.TokenFamily ?? Guid.NewGuid(),
                ExpiryTime = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            }
        );
        await _context.SaveChangesAsync();

        return (rawRefreshToken, hashedRefreshToken);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string rawToken)
    {
        var hashedToken = _jwtService.HashToken(rawToken);
        return await 
            _context
                .RefreshTokens
                .Include(t => t.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Token == rawToken);
    }

    public async Task<RefreshToken?> GetByTokenAndUserAsync(string rawToken, Guid userId)
    {
        var hashedToken = _jwtService.HashToken(rawToken);
        return await _context
            .RefreshTokens
            .Include(t => t.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Token == rawToken && t.UserId == userId);
    }

    public async Task<bool> RevokeAsync(string rawToken)
    {
        var hashedToken = _jwtService.HashToken(rawToken);
        var token = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == rawToken);

        if(token == null)
        {
            return false;
        }

        token.IsRevoked = true;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RevokeFamilyAsync(Guid tokenFamily)
    {
        var familyTokens = await _context.RefreshTokens
            .Where(t => t.Family == tokenFamily && !t.IsRevoked)
            .ToListAsync();

        foreach (var t in familyTokens)
        {
            t.IsRevoked = true;
        }
        await _context.SaveChangesAsync();

        return true;
    }

        public async Task<(string refreshToken, string hashedRefreshToken)?> RenewTokenAsync(RenewRefreshTokenDto dto)
        {
            var existingToken = await _context
                .RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == dto.RawToken && t.UserId == dto.UserId);

            if(existingToken == null)
            {
                return null;
            }
            existingToken.IsRevoked = true;

            var (rawRefreshToken, hashedRefreshToken) = _jwtService.GenerateRefreshToken();
            _context.RefreshTokens.Add
            (
                new RefreshToken{
                    UserId = dto.UserId,
                    Token = hashedRefreshToken,
                    Family =  existingToken.Family,
                    ExpiryTime = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow
                }
            );

            await _context.SaveChangesAsync();

            return (rawRefreshToken, hashedRefreshToken);   
        }
}