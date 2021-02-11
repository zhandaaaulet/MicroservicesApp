﻿using Basket.API.Data.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Data
{
    public class BasketCartContext : IBasketCartContext
    {
        private readonly ConnectionMultiplexer _connection;

        public BasketCartContext(ConnectionMultiplexer connection)
        {
            _connection = connection;
            Redis = _connection.GetDatabase();
        }

        public IDatabase Redis { get; }
    }
}
