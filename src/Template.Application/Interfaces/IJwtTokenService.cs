using Template.Application.Result;
using Template.Core.Models;

namespace Template.Application.Interfaces
{
    public interface IJwtTokenService
    {
        Task<IServiceResult<string>> GenerateTokenAsync(User user, IEnumerable<string> roles, string? oldToken = null);
        IServiceResult<string> GetPropertyFromToken(string? token, string claim);
        Task<IServiceResult<string>> LogoutUser(string token);
    }
}