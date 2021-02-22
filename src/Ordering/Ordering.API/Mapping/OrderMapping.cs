using AutoMapper;
using EventBusRabbitMQ.Events;
using Ordering.API.DTOs;
using Ordering.Core.Entities;
using System;

namespace Ordering.API.Mapping
{
    public class OrderMapping : Profile
    {
        public OrderMapping()
        {
            /*CreateMap<BasketCheckoutEvent, OrderResponse>().ReverseMap();*/
            CreateMap<Order, OrderResponse>();
            CreateMap<BasketCheckoutEvent, Order>();
        }
    }
}
