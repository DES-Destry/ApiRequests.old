using System;
using ApiRequests.Http.Addons;
using ApiRequests.Http.Configuration;
using ApiRequests.Http.Controller;
using Microsoft.Extensions.DependencyInjection;

namespace ApiRequests.Http.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddHttpController<T>(this IServiceCollection services,
            Func<IHttpControllerBuilder<IConfiguration>, IHttpController<IConfiguration>> setupAction = null)
            where T : class, IHttpControllerBuilder<IConfiguration>, new()
        {
            var builder = new T();
            var controller = setupAction != null ? setupAction.Invoke(builder) : builder.Build();

            return services.AddSingleton(controller);
        }

        public static IServiceCollection AddRefreshableHttpController<T>(this IServiceCollection services,
            Func<IRefreshableTokenControllerBuilder<IConfiguration>, IRefreshableTokenController<IConfiguration>>
                setupAction = null) where T : class, IRefreshableTokenControllerBuilder<IConfiguration>, new()
        {
            var builder = new T();
            var controller = setupAction != null ? setupAction.Invoke(builder) : builder.Build();

            return services.AddSingleton(controller);
        }
    }   
}