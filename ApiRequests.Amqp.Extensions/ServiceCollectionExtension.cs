using System;
using ApiRequests.Amqp.Configuration;
using ApiRequests.Amqp.Senders;
using Microsoft.Extensions.DependencyInjection;

namespace ApiRequests.Amqp.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddAmqpSender<TBuilderImplementation>(this IServiceCollection services,
            Func<IAmqpSenderBuilder<IAmqpConfiguration>, IAmqpSender<IAmqpConfiguration>> setupAction = null)
            where TBuilderImplementation : class, IAmqpSenderBuilder<IAmqpConfiguration>, new()
        {
            var builder = new TBuilderImplementation();
            var controller = setupAction != null ? setupAction.Invoke(builder) : builder.Build();

            services.AddSingleton(typeof(TBuilderImplementation), builder);
            return services.AddSingleton(controller);
        }
        
        public static IServiceCollection AddAmqpSender<T, TBuilderImplementation>(this IServiceCollection services,
            Func<IAmqpSenderBuilder<IAmqpConfiguration>, IAmqpSender<IAmqpConfiguration>> setupAction = null)
            where TBuilderImplementation : class, IAmqpSenderBuilder<IAmqpConfiguration>, new()
            where T : IAmqpSender<IAmqpConfiguration>
        {
            var builder = new TBuilderImplementation();
            var controller = setupAction != null ? setupAction.Invoke(builder) : builder.Build();

            services.AddSingleton(typeof(TBuilderImplementation), builder);
            return services.AddSingleton(typeof(T), controller);
        }
    }   
}