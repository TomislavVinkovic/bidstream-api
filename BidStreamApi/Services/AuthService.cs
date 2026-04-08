namespace BidStream.Services;

using BidStream.Common;
using BidStream.Data;
using BidStream.Models.DTOs.Auth;
using BidStream.Models.Entities;
using BidStream.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Mapster;
using BidStream.Models.DTOs.RefreshToken;
using System.Security.Claims;
using BidStream.Extensions;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthService(
        AppDbContext context,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService
    )
    {
        _context = context;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<ServiceResult<UserResponse?>> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
        {
            return ServiceResult<UserResponse?>.Unauthorized("Invalid credentials");
        }

        var accessToken = _jwtService.GenerateAccessToken(user);
        (string rawToken, string hashedToken) = await _refreshTokenService.CreateAsync(new CreateRefreshTokenDto(user.Id));
        
        var userDto = user.ToUserDto(accessToken, rawToken);

        return ServiceResult<UserResponse?>.Ok(new UserResponse(userDto));
    }

    public async Task<ServiceResult<bool>> LogoutAsync(Guid userId, string rawRefreshToken)
    {
        bool revoked = await _refreshTokenService.RevokeAsync(rawRefreshToken);
        if(!revoked)
        {
            return ServiceResult<bool>.Unauthorized();
        }

        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<UserResponse?>> RefreshAsync(TokenRequest request)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        var userId = principal?.GetOptionalUserId().ToString();

        if (string.IsNullOrEmpty(userId))
        {
            return ServiceResult<UserResponse?>.BadRequest("Invalid access token.");
        }
        var stored = await _refreshTokenService.GetByTokenAsync(request.RefreshToken);
        if (stored == null)
        {
            return ServiceResult<UserResponse?>.BadRequest("Invalid refresh token.");
        }

        if (stored.IsRevoked)
        {
            await _refreshTokenService.RevokeFamilyAsync(stored.Family);
            return ServiceResult<UserResponse?>.BadRequest(
                "Token reuse detected. Please log in again."
            );
        }
        if (stored.ExpiryTime <= DateTime.UtcNow)
        {
            return ServiceResult<UserResponse?>.BadRequest("Refresh token expired.");
        }

        var renewDto = new RenewRefreshTokenDto(request.RefreshToken, Guid.Parse(userId));
        var renewResult = await _refreshTokenService.RenewTokenAsync(renewDto);

        if(renewResult == null)
        {
            return ServiceResult<UserResponse?>.BadRequest("Invalid refresh token.");
        }

        var newAccessToken = _jwtService.GenerateAccessToken(stored.User);
        var userDto = stored.User.ToUserDto(newAccessToken, renewResult.Value.refreshToken);
        return ServiceResult<UserResponse?>.Ok(new UserResponse(userDto));
    }

    public async Task<ServiceResult<UserResponse>> RegisterAsync(RegisterDto dto)
    {
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var accessToken = _jwtService.GenerateAccessToken(user);
        var (rawRefreshToken, _) = await _refreshTokenService.CreateAsync(new CreateRefreshTokenDto(user.Id));

        var userDto = user.ToUserDto(accessToken, rawRefreshToken);
        return ServiceResult<UserResponse>.Ok(new UserResponse(userDto));
    }
}