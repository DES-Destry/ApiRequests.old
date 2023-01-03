using ApiRequests.Configuration;
using RabbitMQ.Client;

namespace ApiRequests.Amqp.Configuration;

public interface IAmqpConfiguration : IConfiguration
{
    public string Exchange { get; set; }
    public IBasicProperties BasicProperties { get; set; }
}