using ApiRequests.Configuration;
using ApiRequests.Http.Controller;

namespace ApiRequests.Http.Addons
{
    public interface IRefreshableTokenControllerBuilder<out T> : IHttpControllerBuilder<T> where T : IConfiguration
    {
        IRefreshableTokenControllerBuilder<T> SetRefreshToken(string refreshToken);
    }
}