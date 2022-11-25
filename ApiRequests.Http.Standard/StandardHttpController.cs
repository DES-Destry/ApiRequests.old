using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using ApiRequests.Http.Configuration;
using ApiRequests.Http.Controller;

namespace ApiRequests.Http.Standard
{
    public abstract class StandardHttpController<TConf> : IHttpController<TConf> where TConf : IConfiguration
    {
        private readonly HttpClient _client;
        
        protected readonly List<KeyValuePair<string, string>> QueryParameters;
        protected readonly List<KeyValuePair<string, string>> Headers;

        protected string AccessToken;

        protected object Body;

        protected StandardHttpController()
        {
            _client = new HttpClient();
            
            QueryParameters = new List<KeyValuePair<string, string>>();
            Headers = new List<KeyValuePair<string, string>>();
        }
        
        public abstract TConf Configuration { get; set; }
        
        public void AddQueryParameter(string key, string value)
        {
            var queryParameter = new KeyValuePair<string, string>(key, value);
            
            QueryParameters.RemoveAll(param => param.Key == queryParameter.Key);
            QueryParameters.Add(queryParameter);
        }

        public void AddQueryParameters(IEnumerable<KeyValuePair<string, string>> queryParameters)
        {
            QueryParameters
                .RemoveAll(param => queryParameters
                    .Any(inputParam => inputParam.Key == param.Key));
            
            QueryParameters.AddRange(queryParameters);
        }

        public void SetQueryParameters(IEnumerable<KeyValuePair<string, string>> queryParameters)
        {
            QueryParameters.Clear();
            QueryParameters.AddRange(queryParameters);
        }

        public void RemoveQueryParameters()
        {
            QueryParameters.Clear();
        }

        public void AddHeader(string key, string value)
        {
            var header = new KeyValuePair<string, string>(key, value);
            
            Headers.RemoveAll(param => param.Key == header.Key);
            Headers.Add(header);
        }

        public void AddHeaders(IEnumerable<KeyValuePair<string, string>> headers)
        {
            Headers
                .RemoveAll(param => headers
                    .Any(inputParam => inputParam.Key == param.Key));
            
            Headers.AddRange(headers);
        }

        public void SetHeaders(IEnumerable<KeyValuePair<string, string>> headers)
        {
            Headers.Clear();
            Headers.AddRange(headers);
        }

        public void RemoveHeaders()
        {
            Headers.Clear();
        }

        public void SetAccessToken(string accessToken)
        {
            AccessToken = accessToken;
        }

        public void SetBody(object body)
        {
            Body = body;
        }

        public abstract void SetEnvironment(ServerEnvironment environment);

        public async Task<TO> Get<TO, TE>(string resource) => await HttpRequest<TO, TE>(resource, HttpMethod.Get);

        public async Task<TO> Post<TO, TE>(string resource) => await HttpRequest<TO, TE>(resource, HttpMethod.Post);

        public async Task<TO> Put<TO, TE>(string resource) => await HttpRequest<TO, TE>(resource, HttpMethod.Put);

        public async Task<TO> Patch<TO, TE>(string resource) =>
            await HttpRequest<TO, TE>(resource, new HttpMethod("PATCH"));

        public async Task<TO> Delete<TO, TE>(string resource) => await HttpRequest<TO, TE>(resource, HttpMethod.Delete);
        
        protected abstract void ValidateErrors<TE>(TE response);
        
        private async Task<TO> HttpRequest<TO, TE>(string resource, HttpMethod httpMethod)
        {
            var requestUri = BuildUri(resource);
            var message = BuildMessage(requestUri, httpMethod);
            return await SendMessageAndGetResponse<TO, TE>(message);
        }
        
        private async Task<TO> SendMessageAndGetResponse<TO, TE>(HttpRequestMessage message)
        {
            var response = await _client.SendAsync(message);

            if (response.IsSuccessStatusCode) 
                return await response.Content.ReadFromJsonAsync<TO>();
            
            var error = await response.Content.ReadFromJsonAsync<TE>();
            ValidateErrors(error);

            return await response.Content.ReadFromJsonAsync<TO>();
        }

        // TODO resource can be nullable
        private Uri BuildUri(string resource)
        {
            // TODO: use ??=
            if (resource == null)
                resource = string.Empty;

            var stringBuilder = new StringBuilder();
            
            // TODO: check configuration and tell user, that environment is important to set!!!
            stringBuilder.Append(Configuration.BaseUrl);
            stringBuilder.Append(resource);

            if (QueryParameters != null)
            {
                foreach (var (param, i) in QueryParameters.Select(
                             (param, i) => (param, i)))
                {
                    var separator = i == 0 ? "?" : "&";
                    stringBuilder.Append($"{separator}{param.Key}={param.Value}");
                }
            }

            return new Uri(stringBuilder.ToString());
        }
        
        private HttpRequestMessage BuildMessage(Uri uri, HttpMethod method)
        {
            var message = new HttpRequestMessage()
            {
                Method = method,
                RequestUri = uri
            };

            if (AccessToken != null)
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            if (Headers != null)
            {
                foreach (var header in Headers)
                    message.Headers.Add(header.Key, header.Value);   
            }

            if (Body != null && method != HttpMethod.Get)
                message.Content = JsonContent.Create(Body);
            
            return message;
        }
    }
}