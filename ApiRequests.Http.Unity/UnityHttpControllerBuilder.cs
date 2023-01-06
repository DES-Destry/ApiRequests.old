using System.Collections.Generic;
using ApiRequests.Configuration;
using ApiRequests.Http.Controller;

namespace ApiRequests.Http.Unity
{
    public abstract class UnityHttpControllerBuilder<TConf> : IHttpControllerBuilder<TConf> where TConf : IConfiguration
    {
        protected UnityHttpController<TConf> Controller;

        public IHttpControllerBuilder<TConf> AddQueryParameter(string key, string value)
        {
            Controller.AddQueryParameter(key, value);
            return this;
        }

        public IHttpControllerBuilder<TConf> AddQueryParameters(IEnumerable<KeyValuePair<string, string>> queryParameters)
        {
            Controller.AddQueryParameters(queryParameters);
            return this;
        }

        public IHttpControllerBuilder<TConf> RemoveQueryParameters()
        {
            Controller.RemoveQueryParameters();
            return this;
        }

        public IHttpControllerBuilder<TConf> AddHeader(string key, string value)
        {
            Controller.AddHeader(key, value);
            return this;
        }

        public IHttpControllerBuilder<TConf> AddHeaders(IEnumerable<KeyValuePair<string, string>> headers)
        {
            Controller.AddHeaders(headers);
            return this;
        }

        public IHttpControllerBuilder<TConf> RemoveHeaders()
        {
            Controller.RemoveHeaders();
            return this;
        }

        public IHttpControllerBuilder<TConf> SetAccessToken(string accessToken)
        {
            Controller.SetAccessToken(accessToken);
            return this;
        }
        

        public IHttpControllerBuilder<TConf> SetEnvironment(ServerEnvironment environment)
        {
            Controller.SetEnvironment(environment);
            return this;
        }

        public IHttpController<TConf> Build()
        {
            return Controller;
        }
    }
}