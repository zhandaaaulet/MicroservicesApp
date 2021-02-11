using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Data.Interfaces
{
    public interface IBasketCartContext
    {
        IDatabase Redis { get; }
    }
}
