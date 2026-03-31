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

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;

    public UserService(
        AppDbContext context,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService
    )
    {
        _context = context;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<ServiceResult<UserResponse?>> GetCurrentUserAsync(string currentToken, int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return ServiceResult<UserResponse?>.NotFound();
        }

        var userDto = user.ToUserDto(currentToken);
        return ServiceResult<UserResponse?>.Ok(new UserResponse(userDto));
    }
}