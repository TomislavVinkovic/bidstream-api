using System.Security.Claims;

namespace BidStream.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetRequiredUserId(this ClaimsPrincipal user)
    {
        var id = user.GetOptionalUserId();
        if (id == null)
        {
            throw new UnauthorizedAccessException("User ID not found.");
        }
        return id.Value;
    }

    public static int? GetOptionalUserId(this ClaimsPrincipal user)
    {
        // If the user isn't logged in, ClaimsPrincipal is empty.
        var claim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("id");
        
        if (claim != null && int.TryParse(claim.Value, out int userId))
        {
            return userId;
        }

        return null;
    }
}