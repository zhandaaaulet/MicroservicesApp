using AutoMapper;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Common;
using EventBusRabbitMQ.Events;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Ordering.API.DTOs;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;
using Ordering.Infrastructure.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Ordering.API.RabbitMQ
{
    public class EventBusRabbitMQConsumer
    {
        private readonly IRabbitMQConnection _connection;
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EventBusRabbitMQConsumer(IRabbitMQConnection connection, IMapper mapper, IServiceScopeFactory serviceScopeFactory)
        {
            _connection = connection;
            _mapper = mapper;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Consume()
        {
            var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: EventBusConstants.BasketCheckoutQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += OnReceivedEvent;

            channel.BasicConsume(queue: EventBusConstants.BasketCheckoutQueue, autoAck: true, consumer: consumer);
        }

        private async void OnReceivedEvent(object sender, BasicDeliverEventArgs e)
        {
            if (e.RoutingKey == EventBusConstants.BasketCheckoutQueue)
            {
                var message = Encoding.UTF8.GetString(e.Body.Span);
                var basketCheckoutEvent = JsonConvert.DeserializeObject<BasketCheckoutEvent>(message);

                var orderEntity = _mapper.Map<Order>(basketCheckoutEvent);
                if (orderEntity == null)
                    throw new ApplicationException("Entity could not be mapped.");

                using var scope = _serviceScopeFactory.CreateScope();
                var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                await orderRepository.AddAsync(orderEntity);
            }
        }

        /*private void OnReceivedEvent(object sender, BasicDeliverEventArgs e)
        {
            if (e.RoutingKey == EventBusConstants.BasketCheckoutQueue)
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
                var basketCheckout = JsonConvert.DeserializeObject<BasketCheckoutEvent>(message);

                var scope = _serviceScopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<OrderContext>();
                var order = new Order
                {
                    Username = basketCheckout.Username,
                    TotalPrice = basketCheckout.TotalPrice,

                    FirstName = basketCheckout.FirstName,
                    LastName = basketCheckout.LastName,
                    EmailAddress = basketCheckout.EmailAddress,
                    AddressLine = basketCheckout.AddressLine,
                    Country = basketCheckout.Country,
                    State = basketCheckout.State,
                    ZipCode = basketCheckout.ZipCode,

                    CardName = basketCheckout.CardName,
                    CardNumber = basketCheckout.CardNumber,
                    Expiration = basketCheckout.Expiration,
                    CVV = basketCheckout.CVV,
                    PaymentMethod = basketCheckout.PaymentMethod
                };

                db.Orders.Add(order);
                db.SaveChanges();
            }
        }*/

        public void Disconnect()
        {
            _connection.Dispose();
        }
    }
}
