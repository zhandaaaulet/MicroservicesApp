using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
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

        public BasketController(IBasketCartRepository repository, ILogger<BasketController> logger)
        {
            _repository = repository;
            _logger = logger;
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
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            var basket = await _repository.GetBasket(basketCheckout.Username);
            if(basket == null)
            {
                _logger.LogError("Basket could not be found for user: " + basketCheckout.Username);
                return BadRequest();
            }

            var removed = await _repository.DeleteBasket(basketCheckout.Username);
            if(!removed)
            {
                _logger.LogError("Basket could not be removed for user: " + basketCheckout.Username);
                return BadRequest();
            }

            
        }

    }
}
