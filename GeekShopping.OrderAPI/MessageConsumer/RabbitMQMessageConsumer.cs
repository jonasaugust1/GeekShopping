using GeekShopping.CartAPI.Repository;
using GeekShopping.MessageBus;
using RabbitMQ.Client;
using System.Threading.Channels;

namespace GeekShopping.OrderAPI.MessageConsumer
{
    public class RabbitMQMessageConsumer : BackgroundService
    {
        private readonly OrderRepository _repository;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQMessageConsumer(OrderRepository repository)
        {
            _repository = repository;

            ConnectionFactory factory = new()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare("checkout_queue", false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
