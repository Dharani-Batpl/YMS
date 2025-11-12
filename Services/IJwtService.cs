using System.Threading.Tasks;
using YardManagementApplication.Models;

namespace YardManagementApplication.Services
{
    public interface IJwtService
    {
        string GetAccessToken();
        string GetRefreshToken();
        Task<bool> RefreshTokensAsync();
        Task<bool> IsTokenExpiredAsync();
        void Logout();
      
        void StoreTokensinCookies(TokenResponseModel tokens);
    }
}