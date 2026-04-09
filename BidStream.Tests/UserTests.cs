using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BidStream.Models.DTOs.Auth;
using BidStream.Tests.Extensions;
using FluentAssertions;

namespace BidStream.Tests;

public class UserTests : IClassFixture<BidStreamTestFactory>
{
    private readonly HttpClient _client;

    public UserTests(BidStreamTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCurrentUser_WhenAuthorized_ReturnsUserData()
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

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/users");

        await response.ShouldBeOkAsync();
        
        // Addition: Test that if we remove the token, we get a 401 Unauthorized
        _client.DefaultRequestHeaders.Authorization = null;
        var unauthorizedResponse = await _client.GetAsync("/api/users");
        unauthorizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}