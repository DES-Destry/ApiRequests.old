using System;
using System.Collections.Generic;
using ApiRequests.Amqp.Configuration;
using ApiRequests.Configuration;
using RabbitMQ.Client;

namespace ApiRequests.Amqp.Senders
{
    public interface IAmqpSender<out TConf> where TConf : IAmqpConfiguration
    {
        TConf Configuration { get; }

        void SetExchange(string exchange);
        void SetProperties(IBasicProperties properties);
        void AddMessage(object message);
        void AddMessageRange(IEnumerable<object> message);
        void SetMessages(IEnumerable<object> messages);
        void RemoveMessage(Func<object, bool> removalCondition);
        void ClearMessages();
    
        void SetEnvironment(ServerEnvironment environment);

        void SetCustomConfiguration(IAmqpConfiguration configuration);

        void Publish(string queue);
        void Publish(string queue, string routingKey);
    }   
}