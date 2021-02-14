using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using EventBusRabbitMQ.Common;
using EventBusRabbitMQ.Events;
using EventBusRabbitMQ.Producers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Basket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketCartRepository _repository;
        private readonly ILogger<BasketController> _logger;
        private readonly IMapper _mapper;
        private readonly EventBusRabbitMQProducer _eventBus;

        public BasketController(IBasketCartRepository repository, ILogger<BasketController> logger, IMapper mapper, EventBusRabbitMQProducer eventBus)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _eventBus = eventBus;
        }
        [HttpGet]
        [ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BasketCart>> GetBasket(string username)
        {
            var basket = await _repository.GetBasket(username);
            return Ok(basket ?? new BasketCart(username));
        }

        [HttpPost]
        [ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BasketCart>> UpdateBasket([FromBody] BasketCart basket)
        {
            return Ok(await _repository.UpdateBasket(basket));
        }

        [HttpDelete("{username}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteBasket(string username)
        {
            return Ok(await _repository.DeleteBasket(username));
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            var basket = _repository.GetBasket(basketCheckout.Username);
            if (basket == null)
            {
                _logger.LogError("The Basket doesn't exist for this user " + basketCheckout.Username);
                return BadRequest();
            }
            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.RequestId = Guid.NewGuid();
            eventMessage.TotalPrice = basketCheckout.TotalPrice;
            try
            {
                _eventBus.PublishBasketCheckout(EventBusConstants.BasketCheckoutQueue, eventMessage);

            }
            catch (Exception e)
            {
                _logger.LogError("Error occured during publish " + e.Message);
                throw;
            }

            var removed = await _repository.DeleteBasket(basketCheckout.Username);
            if (!removed)
            {
                Console.WriteLine("Basket cannot be removed for user " + basketCheckout.Username);
                return BadRequest();
            }

            return Accepted();
        }
    }
}
