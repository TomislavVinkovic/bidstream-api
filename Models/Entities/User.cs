using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BidStream.Models.Entities;

[Index(nameof(Username), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
[PrimaryKey("Id")]
public class User : BaseEntity
{
    public Guid Id { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;
    
    [EmailAddress]
    [Required]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;

    public ICollection<Bid> Bids { get; set; } = new List<Bid>();
}