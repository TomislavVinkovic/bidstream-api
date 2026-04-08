using System.Security.Claims;

namespace BidStream.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetRequiredUserId(this ClaimsPrincipal user)
    {
        var id = user.GetOptionalUserId();
        if (id == null)
        {
            throw new UnauthorizedAccessException("User ID not found.");
        }
        return id.Value;
    }

    public static Guid? GetOptionalUserId(this ClaimsPrincipal user)
    {
        // If the user isn't logged in, ClaimsPrincipal is empty.
        var claim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("id");
        
        if (claim != null && Guid.TryParse(claim.Value, out Guid userId))
        {
            return userId;
        }

        return null;
    }
}