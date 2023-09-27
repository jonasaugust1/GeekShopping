using GeekShopping.OrderAPI.Repository;
using GeekShopping.OrderAPI.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.OrderAPI.MessageConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly OrderRepository _repository;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string ExchangeName = "FanoutPaymentUpdateExchange";
        string queueName = "";

        public RabbitMQPaymentConsumer(OrderRepository repository)
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

            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout);
            queueName = _channel.QueueDeclare().QueueName;

            _channel.QueueBind(queueName, ExchangeName, "");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            EventingBasicConsumer consumer = new(_channel);

            consumer.Received += (channel, evt) =>
            {
                string content = Encoding.UTF8.GetString(evt.Body.ToArray());

                UpdatePaymentResultVO? updatedPayment = JsonSerializer
                .Deserialize<UpdatePaymentResultVO>(content);

                if(updatedPayment != null)
                {
                    UpdatePaymentStatus(updatedPayment).GetAwaiter().GetResult();

                    _channel.BasicAck(evt.DeliveryTag, false);
                }
            };

            _channel.BasicConsume(queueName, false, consumer);

            return Task.CompletedTask;
        }

        private async Task UpdatePaymentStatus(UpdatePaymentResultVO updatedPayment)
        {
            try
            {
                await _repository.UpdateOrderStatus(
                    updatedPayment.OrderId, updatedPayment.Status);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in update payment status: {ex.Message}");
            }
        }
    }
}
