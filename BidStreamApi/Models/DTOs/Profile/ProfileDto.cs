namespace BidStream.Models.DTOs.Profile;

public class ProfileDto {
    public string Email { get; set; } = "";
    public string Username { get; set; } = "";
    public string? Bio { get; set; }
    public string? Image { get; set; }
}