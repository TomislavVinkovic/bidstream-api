
using BidStream.Extensions;
using BidStream.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BidStream.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController : ApiControllerBase
{
    private readonly IUserService _userService;
    public UserController(
        IUserService userService
    )
    {
        _userService = userService;
    }

    /// <summary>
    /// Get current user
    /// </summary>
    [HttpGet("")]
    [Authorize]
    public async Task<ActionResult> GetCurrentUser()
    {
        var currentAccessToken = HttpContext.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last() ?? "";
            
        var result = await _userService.GetCurrentUserAsync(currentAccessToken, User.GetRequiredUserId());
        return HandleResult(result);
    }
    
}