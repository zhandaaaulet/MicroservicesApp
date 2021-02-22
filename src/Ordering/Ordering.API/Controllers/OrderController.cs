﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.DTOs;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;

namespace Ordering.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderController(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetOrdersByUsername(string username)
        {
            var orderList = await _orderRepository.GetOrdersByUsername(username);
            var orderResponseList = _mapper.Map<IEnumerable<OrderResponse>>(orderList);
            return Ok(orderResponseList);

            /*var orders = await _orderRepository.GetOrdersByUsername(username);
            if (orders != null) return Ok(_mapper.Map<IEnumerable<OrderResponse>>(orders));
            return NotFound();*/
        }
    }
}
