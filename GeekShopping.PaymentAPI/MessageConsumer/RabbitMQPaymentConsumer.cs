using GeekShopping.PaymentAPI.Messages;
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
        private readonly IProcessorPayment _processPayment;

        public RabbitMQPaymentConsumer(IProcessorPayment processPayment)
        {
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
           
            try
            {
                //_messageSender.SendMessage(payment, "orderpaymentprocessqueue");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error in Process Payment {ex.Message}");
            }
        }
    }
}
