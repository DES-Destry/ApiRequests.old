using ApiRequests.Amqp.Configuration;
using ApiRequests.Amqp.Senders;
using ApiRequests.Configuration;
using RabbitMQ.Client;

namespace ApiRequests.Amqp.Standard
{
    public class StandardAmqpSenderBuilder<TConf> : IAmqpSenderBuilder<TConf> where TConf : IAmqpConfiguration
    {
        protected StandardAmqpSender<TConf> Sender;

        public IAmqpSenderBuilder<TConf> SetExchange(string exchange)
        {
            Sender.SetExchange(exchange);
            return this;
        }

        public IAmqpSenderBuilder<TConf> SetProperties(IBasicProperties properties)
        {
            Sender.SetProperties(properties);
            return this;
        }

        public IAmqpSenderBuilder<TConf> SetEnvironment(ServerEnvironment environment)
        {
            Sender.SetEnvironment(environment);
            return this;
        }


        public IAmqpSender<TConf> Build() => Sender;
    }   
}