using System.Collections.Generic;
using ApiRequests.Configuration;

namespace ApiRequests.Http.Controller
{
    public interface IHttpControllerBuilder<out TConf> where TConf : IConfiguration
    {
        IHttpControllerBuilder<TConf> AddQueryParameter(string key, string value);
        IHttpControllerBuilder<TConf> AddQueryParameters(IEnumerable<KeyValuePair<string, string>> queryParameters);
        IHttpControllerBuilder<TConf> RemoveQueryParameters();
        IHttpControllerBuilder<TConf> AddHeader(string key, string value);
        IHttpControllerBuilder<TConf> AddHeaders(IEnumerable<KeyValuePair<string, string>> headers);
        IHttpControllerBuilder<TConf> RemoveHeaders();
        IHttpControllerBuilder<TConf> SetAccessToken(string accessToken);

        IHttpControllerBuilder<TConf> SetEnvironment(ServerEnvironment environment);
        IHttpControllerBuilder<TConf> SetCustomConfiguration(IConfiguration configuration);

        IHttpController<TConf> Build();
    }
}