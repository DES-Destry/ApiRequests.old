using ApiRequests.Amqp.Configuration;

namespace ApiRequests.Amqp.Senders
{
    public interface IAmqpSenderBuilder<out TConf> where TConf : IAmqpConfiguration
    {
        IAmqpSender<TConf> Build();
    }   
}