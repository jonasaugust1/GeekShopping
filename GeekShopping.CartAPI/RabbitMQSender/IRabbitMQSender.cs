using GeekShopping.MessageBus;

namespace GeekShopping.CartAPI.RabbitMQSender
{
    public interface IRabbitMQSender
    {
        void SendMessage(BaseMessage baseMessage, string queueName);
    }
}
