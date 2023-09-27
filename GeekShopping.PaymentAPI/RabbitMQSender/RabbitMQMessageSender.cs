using GeekShopping.PaymentAPI.Messages;
using GeekShopping.MessageBus;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace GeekShopping.PaymentAPI.RabbitMQSender
{
    public class RabbitMQMessageSender : IRabbitMQMessageSender
    {
        private readonly string _hostName;
        private readonly string _password;
        private readonly string _userName;
        private IConnection _connection;
        private const string ExchangeName = "FanoutPaymentUpdateExchange";

        public RabbitMQMessageSender()
        {
            _hostName = "localhost";
            _password = "guest";
            _userName = "guest";
        }

        public void SendMessage(BaseMessage baseMessage)
        {
            if (ConnectionExists())
            {
                using IModel? channel = _connection.CreateModel();

                if (channel != null)
                {
                    channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout, durable: false);

                    byte[] body = GetMessageAsByteArray(baseMessage);

                    channel.BasicPublish(
                        exchange: ExchangeName,
                        routingKey: "",
                        basicProperties: null,
                        body: body
                        );
                }
            }
        }

        private static byte[] GetMessageAsByteArray(BaseMessage baseMessage)
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = true,
            };

            string json = JsonSerializer.Serialize((UpdatePaymentResultMessage)baseMessage, options);

            byte[] body = Encoding.UTF8.GetBytes(json);
            return body;
        }

        private void CreateConnection()
        {
            try
            {
                ConnectionFactory factory = new()
                {
                    HostName = _hostName,
                    UserName = _userName,
                    Password = _password,
                };

                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                Console.Write($"Error in Creating MQ Connection {ex.Message}");
            }
        }

        private bool ConnectionExists()
        {
            if(_connection != null ) return true;

            CreateConnection();

            return _connection != null;
        }
    }
}
