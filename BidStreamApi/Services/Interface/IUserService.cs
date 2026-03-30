using BidStream.Common;
using BidStream.Models.DTOs.Auth;

namespace BidStream.Services.Interface;

public interface IUserService
{
    Task<ServiceResult<UserResponse?>> LoginAsync(LoginDto dto);
    Task<ServiceResult<bool>> LogoutAsync(int userId, string rawRefreshToken);
    Task<ServiceResult<UserResponse>> RegisterAsync(RegisterDto dto);
    Task<ServiceResult<UserResponse?>> RefreshAsync(TokenRequest request);
    Task<ServiceResult<UserResponse?>> GetCurrentUserAsync(string currentToken, int userId);
}