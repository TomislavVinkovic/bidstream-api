namespace BidStream.Models.DTOs.RefreshToken;

public class RenewRefreshTokenDto
{
    public RenewRefreshTokenDto(string rawToken, Guid userId)
    {
        UserId = userId;
        RawToken = rawToken;
    }
    public Guid UserId { get; set; }
    public string RawToken { get; set; }
}