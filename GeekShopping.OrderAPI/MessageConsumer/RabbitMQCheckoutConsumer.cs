using GeekShopping.CartAPI.Repository;
using GeekShopping.MessageBus;
using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace GeekShopping.OrderAPI.MessageConsumer
{
    public class RabbitMQCheckoutConsumer : BackgroundService
    {
        private readonly OrderRepository _repository;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQCheckoutConsumer(OrderRepository repository)
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
            stoppingToken.ThrowIfCancellationRequested();

            EventingBasicConsumer consumer = new(_channel);

            consumer.Received += (channel, evt) =>
            {
                string content = Encoding.UTF8.GetString(evt.Body.ToArray());

                CheckoutHeaderVO? checkoutHeader = JsonSerializer
                .Deserialize<CheckoutHeaderVO>(content);

                if(checkoutHeader != null)
                {
                    ProcessOrder(checkoutHeader).GetAwaiter().GetResult();

                    _channel.BasicAck(evt.DeliveryTag, false);
                }
            };

            _channel.BasicConsume("checkout_queue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task ProcessOrder(CheckoutHeaderVO checkoutHeader)
        {
            OrderHeader orderHeader = new()
            {
                UserId = checkoutHeader.UserId,
                FirstName = checkoutHeader.FirstName,
                LastName = checkoutHeader.LastName,
                OrderDetails = new List<OrderDetail>(),
                CardNumber = checkoutHeader.CardNumber,
                CouponCode = checkoutHeader.CouponCode,
                CVV = checkoutHeader.CVV,
                DiscountAmount = checkoutHeader.DiscountAmount,
                Email = checkoutHeader.Email,
                ExpiryDate = checkoutHeader.ExpiryDate,
                OrderTime = DateTime.Now,
                PaymentStatus = false,
                PhoneNumber = checkoutHeader.PhoneNumber,
                PurchaseDate = checkoutHeader.PurchaseDate,
            };

            foreach(CartDetailVO cartDetail in checkoutHeader.CartDetails)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cartDetail.ProductId,
                    ProductName = cartDetail.Product.Name,
                    Price = cartDetail.Product.Price,
                    Count = cartDetail.Count,
                };

                orderHeader.TotalItens += orderDetail.Count;
                orderHeader.OrderDetails.Add(orderDetail);
            }

            await _repository.AddOrder(orderHeader);
        }
    }
}
