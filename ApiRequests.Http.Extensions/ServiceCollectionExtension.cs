using System;
using ApiRequests.Http.Configuration;
using ApiRequests.Http.Controller;
using Microsoft.Extensions.DependencyInjection;

namespace ApiRequests.Http.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddHttpController<T>(this IServiceCollection services,
            Func<IHttpControllerBuilder<IConfiguration>, IHttpController<IConfiguration>> setupAction)
            where T : class, IHttpControllerBuilder<IConfiguration>, new()
        {
            var builder = new T();
            var controller = setupAction.Invoke(builder);
            
            return services.AddSingleton(controller);
        }
    }   
}