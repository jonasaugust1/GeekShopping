using GeekShopping.MessageBus;
using RabbitMQ.Client;

namespace GeekShopping.CartAPI.RabbitMQSender
{
    public class RabbitMQMessageSender : IRabbitMQSender
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
            ConnectionFactory factory = new()
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password,
            };

            _connection = factory.CreateConnection();

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

        private byte[] GetMessageAsByteArray(BaseMessage baseMessage)
        {
            throw new NotImplementedException();
        }
    }
}
