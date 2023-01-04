using ApiRequests.Configuration;
using RabbitMQ.Client;

namespace ApiRequests.Amqp.Configuration
{
    public interface IAmqpConfiguration : IConfiguration
    {
        string Exchange { get; set; }
        IBasicProperties BasicProperties { get; set; }
    }   
}