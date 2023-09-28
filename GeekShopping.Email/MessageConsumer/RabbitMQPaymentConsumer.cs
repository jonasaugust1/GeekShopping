using GeekShopping.Email.Messages;
using GeekShopping.Email.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.Email.MessageConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly EmailRepository _repository;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string ExchangeName = "FanoutPaymentUpdateExchange";
        string queueName = "";

        public RabbitMQPaymentConsumer(EmailRepository repository)
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

                UpdatePaymentResultMessage? message = JsonSerializer
                .Deserialize<UpdatePaymentResultMessage>(content);

                if(message != null)
                {
                    ProcessLogs(message).GetAwaiter().GetResult();

                    _channel.BasicAck(evt.DeliveryTag, false);
                }
            };

            _channel.BasicConsume(queueName, false, consumer);

            return Task.CompletedTask;
        }

        private async Task ProcessLogs(UpdatePaymentResultMessage message)
        {
            try
            {
                await _repository.LogEmail(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in update payment status: {ex.Message}");
            }
        }
    }
}
