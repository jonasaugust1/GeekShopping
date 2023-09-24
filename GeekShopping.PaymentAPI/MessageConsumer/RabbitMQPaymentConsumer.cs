using GeekShopping.PaymentAPI.Messages;
using GeekShopping.PaymentAPI.RabbitMQSender;
using GeekShopping.PaymentProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.PaymentAPI.MessageConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private IRabbitMQMessageSender _messageSender;
        private readonly IProcessorPayment _processPayment;

        public RabbitMQPaymentConsumer(
            IRabbitMQMessageSender messageSender, IProcessorPayment processPayment)
        {
            _messageSender = messageSender;
            _processPayment = processPayment;

            ConnectionFactory factory = new()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare("order_payment_process_queue", false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            EventingBasicConsumer consumer = new(_channel);

            consumer.Received += (channel, evt) =>
            {
                string content = Encoding.UTF8.GetString(evt.Body.ToArray());

                PaymentMessage? paymentMessage = JsonSerializer
                .Deserialize<PaymentMessage>(content);

                if(paymentMessage != null)
                {
                    ProcessPayment(paymentMessage).GetAwaiter().GetResult();

                    _channel.BasicAck(evt.DeliveryTag, false);
                }
            };

            _channel.BasicConsume("order_payment_process_queue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task ProcessPayment(PaymentMessage paymentMessage)
        {
            bool result = _processPayment.PaymentProcessor();

            UpdatePaymentResultMessage paymentResult = new()
            {
                Status = result,
                OrderId = paymentMessage.OrderId,
                Email = paymentMessage.Email,
            };

            try
            {
                _messageSender.SendMessage(paymentResult, "order_payment_result_queue");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error in Process Payment {ex.Message}");
            }
        }
    }
}
