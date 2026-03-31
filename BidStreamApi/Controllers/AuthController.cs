
using BidStream.Extensions;
using BidStream.Models.DTOs.Auth;
using BidStream.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BidStream.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(
        IAuthService authService
    )
    {
        _authService = authService;
    }

    /// <summary>
    /// User login
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.user);
        return HandleResult(result);
    }

    /// <summary>
    /// User registration
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request.user);
        return HandleResult(result);
    }

    /// <summary>
    /// The endpoint for refreshing jwt tokens
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh(TokenRequest request)
    {
        var result = await _authService.RefreshAsync(request);
        return HandleResult(result);
    }

    /// <summary>
    /// User log out
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout(TokenRequest request)
    {
        var result = await _authService.LogoutAsync(User.GetRequiredUserId(), request.RefreshToken);
        return HandleResult(result);
    }
    
}