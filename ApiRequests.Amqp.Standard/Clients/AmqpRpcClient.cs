using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ApiRequests.Amqp.Standard.Clients
{
    internal class AmqpRpcClient : IDisposable
    {
        private readonly string _exchange;
        private readonly string _queue;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IBasicProperties _props;

        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _callbackMapper =
            new ConcurrentDictionary<string, TaskCompletionSource<string>>();

        public AmqpRpcClient(IConnectionFactory factory, IBasicProperties props, string queue, string exchange = "")
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _exchange = exchange;
            _queue = queue;
            _props = props ?? _channel.CreateBasicProperties();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += OnMessageReceived;

            _channel.BasicConsume(consumer, _props.ReplyTo ?? "ApiRequests.Amqp.Standard_reply-to", autoAck: true);
        }

        public Task<string> CallAsync(string correlationId, ReadOnlyMemory<byte> body)
        {
            _props.CorrelationId = correlationId;

            var tsc = new TaskCompletionSource<string>();
            _callbackMapper.TryAdd(correlationId, tsc);

            _channel.BasicPublish(_exchange, _queue, _props, body);

            return tsc.Task;
        }
        
        public void Dispose()
        {
            _connection.Close();
        }

        private void OnMessageReceived(object sender, BasicDeliverEventArgs args)
        {
            if (!_callbackMapper.TryRemove(args.BasicProperties.CorrelationId, out var tcs))
                return;

            var body = args.Body.ToArray();
            var response = Encoding.UTF8.GetString(body);

            tcs.TrySetResult(response);
        }
    }
}