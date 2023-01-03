using System;
using System.Threading.Tasks;
using ApiRequests.Configuration;
using ApiRequests.Http.Controller;

namespace ApiRequests.Http.Addons
{
    public interface IRefreshableTokenController<out T> : IHttpController<T> where T : IConfiguration
    {
        string RefreshToken { get; }

        event Action<string, string> OnTokenRefreshing;
        
        void SetRefreshToken(string refreshToken);
        Task RefreshTokens();
    }
}