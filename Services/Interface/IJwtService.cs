using System.Security.Claims;
using BidStream.Models.Entities;

namespace BidStream.Services.Interface;

public interface IJwtService
{
    public string GenerateAccessToken(User user);
    public (string rawToken, string hashedToken) GenerateRefreshToken();

    public string HashToken(string token);
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}