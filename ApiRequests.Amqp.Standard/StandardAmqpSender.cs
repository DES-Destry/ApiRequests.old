using System;
using System.Text;
using System.Text.Json;
using ApiRequests.Amqp.Configuration;
using ApiRequests.Amqp.Senders;
using ApiRequests.Configuration;
using RabbitMQ.Client;

namespace ApiRequests.Amqp.Standard
{
    public abstract class StandardAmqpSender<TConf> : IAmqpSender<TConf> where TConf : IAmqpConfiguration
    {
        protected ConnectionFactory AmqpConnectionFactory;
        
        protected string Exchange;
        protected IBasicProperties Properties;
        protected object[] Messages;
    
        public abstract TConf Configuration { get; }

        protected StandardAmqpSender()
        {
            AmqpConnectionFactory = new ConnectionFactory();

            Exchange = string.Empty;
            Messages = Array.Empty<object>();
        }

        public void SetExchange(string exchange) => Exchange = exchange;
        public void SetProperties(IBasicProperties properties) => Properties = properties;
        public void SetMessage(object message) => Messages[0] = message;
        public void SetMessages(object[] messages) => Messages = messages;

        public abstract void SetEnvironment(ServerEnvironment environment);

        public void Publish(string routingKey)
        {
            using (var amqpConnection = AmqpConnectionFactory.CreateConnection())
            {
                using (var amqpChannel = amqpConnection.CreateModel())
                {
                    var body = BuildBody(Messages[0]);
                    amqpChannel.BasicPublish(Exchange, routingKey, basicProperties: Properties, body: body);
                }   
            }
        }

        private ReadOnlyMemory<byte> BuildBody(object message)
        {
            var jsonMessage = JsonSerializer.Serialize(message ?? "null");
            return Encoding.UTF8.GetBytes(jsonMessage);
        }
    }   
}