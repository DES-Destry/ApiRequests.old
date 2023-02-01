using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiRequests.Configuration;

namespace ApiRequests.Http.Controller
{
    public interface IHttpController<out TConf> where TConf : IConfiguration
    {
        TConf Configuration { get; }
        
        // TODO: AddQueryArray(string key, IEnumerable<string> elements)
        void AddQueryParameter(string key, string value);
        void AddQueryParameters(IEnumerable<KeyValuePair<string, string>> queryParameters);
        void SetQueryParameters(IEnumerable<KeyValuePair<string, string>> queryParameters);
        void RemoveQueryParameters();
        void AddHeader(string key, string value);
        void AddHeaders(IEnumerable<KeyValuePair<string, string>> headers);
        void SetHeaders(IEnumerable<KeyValuePair<string, string>> headers);
        void RemoveHeaders();
        void SetAccessToken(string accessToken);
        void SetBody(object body);

        void SetEnvironment(ServerEnvironment environment);
        [Obsolete("Better to use SetEnvironment method to configure request sending")]
        void SetCustomConfiguration(IConfiguration configuration);

        Task<TO> Get<TO, TE>(string resource);
        Task<TO> Post<TO, TE>(string resource);
        Task<TO> Put<TO, TE>(string resource);
        Task<TO> Patch<TO, TE>(string resource);
        Task<TO> Delete<TO, TE>(string resource);
    }
}