using System;
using System.Collections.Generic;
using System.Linq;
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
        protected List<object> Messages;
    
        public abstract TConf Configuration { get; set; }

        protected StandardAmqpSender()
        {
            AmqpConnectionFactory = new ConnectionFactory();

            Exchange = string.Empty;
            Messages = new List<object>();
        }

        public void SetExchange(string exchange) => Exchange = exchange;
        public void SetProperties(IBasicProperties properties) => Properties = properties;
        
        public void AddMessage(object message) => Messages.Add(message);
        public void AddMessageRange(IEnumerable<object> messages) => Messages.AddRange(messages);
        public void SetMessages(IEnumerable<object> messages) => Messages = messages.ToList();
        public void RemoveMessage(Func<object, bool> removalCondition)
        {
            Messages = Messages.Where(message => !removalCondition.Invoke(message)).ToList();
        }
        public void ClearMessages() => Messages.Clear();

        public abstract void SetEnvironment(ServerEnvironment environment);


        public void Publish(string queue)
        {
            using var amqpConnection = AmqpConnectionFactory.CreateConnection();
            using var amqpChannel = amqpConnection.CreateModel();
            
            foreach (var body in Messages.Select(BuildBody))
                amqpChannel.BasicPublish(Exchange, queue, basicProperties: Properties, body: body);
        }

        public void Publish(string queue, string routingKey)
        {
            using var amqpConnection = AmqpConnectionFactory.CreateConnection();
            using var amqpChannel = amqpConnection.CreateModel();

            foreach (var body in Messages
                         .Select(message => BuildRoutingMessage(message, routingKey))
                         .Select(BuildBody))
                amqpChannel.BasicPublish(Exchange, queue, basicProperties: Properties, body: body);
        }

        private object BuildRoutingMessage(object message, string routingKey)
        {
            return new
            {
                Id = Guid.NewGuid().ToString(),
                Pattern = routingKey,
                Data = message,
            };
        }

        private ReadOnlyMemory<byte> BuildBody(object message)
        {
            var jsonMessage = JsonSerializer.Serialize(message ?? "null");
            return Encoding.UTF8.GetBytes(jsonMessage);
        }
    }   
}