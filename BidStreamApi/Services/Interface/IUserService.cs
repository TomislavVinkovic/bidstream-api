using BidStream.Common;
using BidStream.Models.DTOs.Auth;

namespace BidStream.Services.Interface;

public interface IUserService
{
    Task<ServiceResult<UserResponse?>> GetCurrentUserAsync(string currentToken, int userId);
}