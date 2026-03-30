namespace BidStream.Models.DTOs.Auth;

public record TokenRequest(string AccessToken, string RefreshToken);