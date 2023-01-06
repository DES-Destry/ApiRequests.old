using System;
using ApiRequests.Configuration;
using ApiRequests.Http.Addons;
using ApiRequests.Http.Controller;
using Microsoft.Extensions.DependencyInjection;

namespace ApiRequests.Http.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddHttpController<TBuilderImplementation>(this IServiceCollection services,
            Func<IHttpControllerBuilder<IConfiguration>, IHttpController<IConfiguration>> setupAction = null)
            where TBuilderImplementation : class, IHttpControllerBuilder<IConfiguration>, new()
        {
            var builder = new TBuilderImplementation();
            var controller = setupAction != null ? setupAction.Invoke(builder) : builder.Build();

            services.AddSingleton(typeof(TBuilderImplementation), builder);
            return services.AddSingleton(controller);
        }
        
        public static IServiceCollection AddHttpController<T, TBuilderImplementation>(this IServiceCollection services,
            Func<IHttpControllerBuilder<IConfiguration>, IHttpController<IConfiguration>> setupAction = null)
            where TBuilderImplementation : class, IHttpControllerBuilder<IConfiguration>, new()
            where T : class, IHttpController<IConfiguration>
        {
            var builder = new TBuilderImplementation();
            var controller = (setupAction != null ? setupAction.Invoke(builder) : builder.Build()) as T;

            services.AddSingleton(typeof(TBuilderImplementation), builder);
            return services.AddSingleton(typeof(T), controller);
        }

        public static IServiceCollection AddRefreshableHttpController<TBuilderImplementation>(this IServiceCollection services,
            Func<IRefreshableTokenControllerBuilder<IConfiguration>, IRefreshableTokenController<IConfiguration>>
                setupAction = null) where TBuilderImplementation : class, IRefreshableTokenControllerBuilder<IConfiguration>, new()
        {
            var builder = new TBuilderImplementation();
            var controller =
                (setupAction != null ? setupAction.Invoke(builder) : builder.Build()) as
                IRefreshableTokenController<IConfiguration>;

            services.AddSingleton(typeof(TBuilderImplementation), builder);
            return services.AddSingleton(controller);
        }

        public static IServiceCollection AddRefreshableHttpController<T, TBuilderImplementation>(
            this IServiceCollection services,
            Func<IRefreshableTokenControllerBuilder<IConfiguration>, IRefreshableTokenController<IConfiguration>>
                setupAction = null)
            where TBuilderImplementation : class, IRefreshableTokenControllerBuilder<IConfiguration>, new()
            where T : class, IRefreshableTokenController<IConfiguration>
        {
            var builder = new TBuilderImplementation();
            var controller =
                (setupAction != null ? setupAction.Invoke(builder) : builder.Build()) as T;

            services.AddSingleton(typeof(TBuilderImplementation), builder);
            return services.AddSingleton(typeof(T), controller);
        }
    }   
}