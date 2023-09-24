using GeekShopping.CartAPI.Messages;
using GeekShopping.MessageBus;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace GeekShopping.CartAPI.RabbitMQSender
{
    public class RabbitMQMessageSender : IRabbitMQMessageSender
    {
        private readonly string _hostName;
        private readonly string _password;
        private readonly string _userName;
        private IConnection _connection;

        public RabbitMQMessageSender()
        {
            _hostName = "localhost";
            _password = "guest";
            _userName = "guest";
        }

        public void SendMessage(BaseMessage baseMessage, string queueName)
        {
            

            using IModel? channel = _connection.CreateModel();

            if(channel != null)
            {
                channel.QueueDeclare(queueName, false, false, false, arguments: null);

                byte[] body = GetMessageAsByteArray(baseMessage);

                channel.BasicPublish(
                    exchange: "",
                    routingKey: queueName,
                    basicProperties: null,
                    body: body
                    );
            }
        }

        private static byte[] GetMessageAsByteArray(BaseMessage baseMessage)
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = true,
            };

            string json = JsonSerializer.Serialize((CheckoutHeaderVO)baseMessage, options);

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
