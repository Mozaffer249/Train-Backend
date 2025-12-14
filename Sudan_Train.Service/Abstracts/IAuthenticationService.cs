using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Data.Results;
using System.IdentityModel.Tokens.Jwt;

namespace Sudan_Train.Service.Abstracts
{
    public interface IAuthenticationService
    {
        Task<JwtAuthResult> GetJWTToken(User user);
        Task<JwtSecurityToken?> ReadJWTToken(string accessToken);
        Task<JwtAuthResult?> GetRefreshToken(User user, JwtSecurityToken jwtToken, string refreshToken);
        Task<string> ValidateToken(string accessToken);
        Task<string> ConfirmEmail(int userId, string code);
        Task<string> SendResetPasswordCode(string email);
        Task<string> ResetPassword(string email, string code, string newPassword);
        Task<bool> RevokeTokenAsync(string accessToken, string? refreshToken, int userId, bool allDevices);
    }
}
