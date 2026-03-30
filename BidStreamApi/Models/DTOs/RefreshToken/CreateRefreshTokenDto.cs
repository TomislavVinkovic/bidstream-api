namespace BidStream.Models.DTOs.RefreshToken;

public class CreateRefreshTokenDto
{
    public CreateRefreshTokenDto(Guid userId, Guid? tokenFamily = null)
    {
        UserId = userId;
        TokenFamily = tokenFamily;
    }
    public Guid UserId { get; set; }
    public Guid? TokenFamily { get; set; }
}