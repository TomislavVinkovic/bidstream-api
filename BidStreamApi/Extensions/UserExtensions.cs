using BidStream.Models.DTOs.Auth;
using BidStream.Models.Entities;
using Mapster;

namespace BidStream.Extensions;

public static class UserExtensions
{
    public static UserDto ToUserDto(this User user, string accessToken, string refreshToken = "")
    {
        var userDto = user.Adapt<UserDto>();
        userDto.Token = accessToken;
        userDto.RefreshToken = refreshToken == "" ? string.Empty : refreshToken;
        return userDto;
    }
}