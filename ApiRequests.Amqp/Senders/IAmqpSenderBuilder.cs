using ApiRequests.Amqp.Configuration;
using ApiRequests.Configuration;
using RabbitMQ.Client;

namespace ApiRequests.Amqp.Senders
{
    public interface IAmqpSenderBuilder<out TConf> where TConf : IAmqpConfiguration
    {
        IAmqpSenderBuilder<TConf> SetExchange(string exchange);
        IAmqpSenderBuilder<TConf> SetProperties(IBasicProperties properties);
        IAmqpSenderBuilder<TConf> SetEnvironment(ServerEnvironment environment);
        IAmqpSenderBuilder<TConf> SetCustomConfiguration(IAmqpConfiguration configuration);

        IAmqpSender<TConf> Build();
    }   
}