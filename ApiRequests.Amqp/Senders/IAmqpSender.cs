using System;
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
        void AddMessageRange(object[] message);
        void SetMessages(object[] messages);
        void RemoveMessage(Func<object, bool> removalCondition);
        void ClearMessages();
    
        void SetEnvironment(ServerEnvironment environment);

        void Publish(string routingKey);
    }   
}