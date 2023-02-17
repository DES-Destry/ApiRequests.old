using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiRequests.Configuration;
using ApiRequests.Http.Controller;
using ApiRequests.Http.Unity.Exceptions;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace ApiRequests.Http.Unity
{
    public abstract class UnityHttpController<TConf> : IHttpController<TConf> where TConf : IConfiguration
    {
        protected readonly List<KeyValuePair<string, string>> QueryParameters;
        protected readonly List<KeyValuePair<string, string>> Headers;

        protected string AccessToken;

        protected object Body;

        protected UnityHttpController()
        {
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
        public void SetCustomConfiguration(IConfiguration configuration)
        {
            if (configuration is TConf tConf) Configuration = tConf;
        }

        public async Task<TO> Get<TO, TE>(string resource)
        {
            var uri = BuildUri(resource);

            using (var request = FormRequest(uri, "GET"))
            {
                var operation = request.SendWebRequest();
                return await GetDataFromRequestOperation<TO, TE>(operation);
            }
        }

        public async Task<TO> Post<TO, TE>(string resource)
        {
            var uri = BuildUri(resource);

            using (var request = FormRequest(uri, "POST"))
            {
                var operation = request.SendWebRequest();
                return await GetDataFromRequestOperation<TO, TE>(operation);
            }
        }

        public async Task<TO> Put<TO, TE>(string resource)
        {
            var uri = BuildUri(resource);

            using (var request = FormRequest(uri, "PUT"))
            {
                var operation = request.SendWebRequest();
                return await GetDataFromRequestOperation<TO, TE>(operation);
            }
        }

        public async Task<TO> Patch<TO, TE>(string resource)
        {
            var uri = BuildUri(resource);

            using (var request = FormRequest(uri, "PATCH"))
            {
                var operation = request.SendWebRequest();
                return await GetDataFromRequestOperation<TO, TE>(operation);
            }
        }

        public async Task<TO> Delete<TO, TE>(string resource)
        {
            var uri = BuildUri(resource);

            using (var request = FormRequest(uri, "DELETE"))
            {
                var operation = request.SendWebRequest();
                return await GetDataFromRequestOperation<TO, TE>(operation);
            }
        }

        protected abstract void ValidateErrors<TE>(TE response);

        private UnityWebRequest FormRequest(Uri uri, string httpMethod)
        {
            var request = new UnityWebRequest(uri);
            request.method = httpMethod;
            
            request.downloadHandler = new DownloadHandlerBuffer();
            
            foreach (var header in Headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }
            
            request.SetRequestHeader("Content-Type","application/json");
            
            if (AccessToken != null) request.SetRequestHeader("Authorization", $"Bearer {AccessToken}");

            if (httpMethod != "GET" && Body != null)
            {
                var jsonData = JsonConvert.SerializeObject(Body);
                var bodyBytes = new UTF8Encoding().GetBytes(jsonData);

                request.uploadHandler = new UploadHandlerRaw(bodyBytes);
            }

            return request;
        }

        private async Task<TO> GetDataFromRequestOperation<TO, TE>(UnityWebRequestAsyncOperation operation)
        {
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (operation.webRequest.result == UnityWebRequest.Result.Success)
                return JsonConvert.DeserializeObject<TO>(operation.webRequest.downloadHandler.text);

            if (operation.webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                var response = JsonConvert.DeserializeObject<TE>(operation.webRequest.downloadHandler.text);
                ValidateErrors(response);
            }

            throw new UnityWebRequestException(operation.webRequest.result);
        }
        
        private Uri BuildUri(string resource)
        {
            if (resource == null)
                resource = string.Empty;

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(Configuration.BaseUrl);
            stringBuilder.Append(resource);

            if (QueryParameters == null) return new Uri(stringBuilder.ToString());
            
            foreach (var (param, i) in QueryParameters.Select(
                         (param, i) => (param, i)))
            {
                var separator = i == 0 ? "?" : "&";
                stringBuilder.Append($"{separator}{param.Key}={param.Value}");
            }

            return new Uri(stringBuilder.ToString());
        }
    }
}