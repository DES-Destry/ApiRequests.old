using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ApiRequests.Amqp.Configuration;
using ApiRequests.Amqp.Senders;
using ApiRequests.Amqp.Standard.Clients;
using ApiRequests.Amqp.Standard.Dto;
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
        public void SetMessage(object message) => Messages = new List<object> {message};
        public void SetMessages(IEnumerable<object> messages) => Messages = messages.ToList();
        public void RemoveMessage(Func<object, bool> removalCondition)
        {
            Messages = Messages.Where(message => !removalCondition.Invoke(message)).ToList();
        }
        public void ClearMessages() => Messages.Clear();

        public abstract void SetEnvironment(ServerEnvironment environment);

        public void SetCustomConfiguration(IAmqpConfiguration configuration)
        {
            if (configuration is TConf tConf) Configuration = tConf;
        }


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
        
        public async Task<T> Request<T>(string queue)
        {
            using var client = new AmqpRpcClient(AmqpConnectionFactory, Configuration?.BasicProperties, queue, Exchange);
            var correlationId = Guid.NewGuid().ToString();

            if (Messages.Count <= 0)
                throw new ArgumentException("Call .SetMessage(object) method to set message to send!");

            var body = BuildBody(Messages[0]);
            var reply = await client.CallAsync(correlationId, body);

            var rabbitResponse = JsonSerializer.Deserialize<RabbitResponseDto<T>>(reply);

            return rabbitResponse.Response.Data;
        }
        
        public async Task<T> Request<T>(string queue, string routingKey)
        {
            using var client = new AmqpRpcClient(AmqpConnectionFactory, Configuration?.BasicProperties, queue, Exchange);
            var correlationId = Guid.NewGuid().ToString();

            if (Messages.Count <= 0)
                throw new ArgumentException("Call .SetMessage(object) method to set message to send!");

            var body = BuildBody(BuildRoutingMessage(correlationId, Messages[0], routingKey));
            var reply = await client.CallAsync(correlationId, body);

            var rabbitResponse = JsonSerializer.Deserialize<RabbitResponseDto<T>>(reply);

            return rabbitResponse.Response.Data;
        }
        
        private static object BuildRoutingMessage(object message, string routingKey)
        {
            // Nest.js specific message template
            return new
            {
                Id = Guid.NewGuid().ToString(),
                Pattern = routingKey,
                Data = message,
            };
        }
        
        private static object BuildRoutingMessage(string correlationId, object message, string routingKey)
        {
            // Nest.js specific message template
            return new
            {
                Id = correlationId,
                Pattern = routingKey,
                Data = message,
            };
        }

        private static ReadOnlyMemory<byte> BuildBody(object message)
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            var jsonMessage = JsonSerializer.Serialize(message ?? "null", jsonOptions);
            return Encoding.UTF8.GetBytes(jsonMessage);
        }
    }   
}