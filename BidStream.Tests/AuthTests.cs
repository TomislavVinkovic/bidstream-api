using System.Net;
using System.Net.Http.Json;
using BidStream.Models.DTOs.Auth;
using BidStream.Tests.Extensions;
using FluentAssertions;

namespace BidStream.Tests;

public class AuthTests : IClassFixture<BidStreamTestFactory>
{
    private readonly HttpClient _client;

    public AuthTests(BidStreamTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsTokens()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "TestUser",
            Email = "testuser@example.com",
            Password = "SecurePassword123!"
        };
        var request = new RegisterRequest(registerDto);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        await response.ShouldBeOkAsync();
        
        var result = await response.Content.ReadFromJsonAsync<UserResponse>();
        result.Should().NotBeNull();
        result.user.Token.Should().NotBeNullOrEmpty();
        result.user.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOk()
    {
        // Arrange: Create the user first
        var credentials = new { Username = "LoginUser", Email = "loginuser@example.com", Password = "Password123!" };
        var registerDto = new RegisterDto 
        { 
            Username = credentials.Username, 
            Email = credentials.Email, 
            Password = credentials.Password 
        };
        var registerRequest = new RegisterRequest(registerDto);
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Act: Try to log in
        var loginDto = new LoginDto
        {
            Email = credentials.Email,
            Password = credentials.Password
        };
        var loginRequest = new LoginRequest(loginDto);
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        await response.ShouldBeOkAsync();
        var result = await response.Content.ReadFromJsonAsync<UserResponse>();
        result!.user.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Refresh_WhenValidRefreshToken_ReturnsNewToken()
    {
        var credentials = new { 
            Username = "AuthUser", 
            Email = "authuser@example.com", 
            Password = "Password123!" 
        };

        var registerDto = new RegisterDto 
        { 
            Username = credentials.Username, 
            Email = credentials.Email, 
            Password = credentials.Password 
        };

        var registerRequest = new RegisterRequest(registerDto);

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        
        var authData = await registerResponse.Content.ReadFromJsonAsync<UserResponse>();

        var token = authData!.user.Token;
        var refreshToken = authData!.user.RefreshToken;
        var refreshRequest = new TokenRequest(AccessToken: token, RefreshToken: refreshToken);

        var response = await _client.PostAsJsonAsync("/api/auth/refresh", refreshRequest);

        await response.ShouldBeOkAsync();

        var result = await response.Content.ReadFromJsonAsync<UserResponse>();

        result!.user.Token.Should().NotBeNullOrEmpty();
        result!.user.RefreshToken.Should().NotBeNullOrEmpty();
    }
}