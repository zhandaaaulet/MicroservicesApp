using Microsoft.EntityFrameworkCore;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(OrderContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Order>> GetOrdersByUsername(string username)
        {
            var listOfOrders = await _dbContext.Orders.Where(i => i.UserName == username).ToListAsync();
            return listOfOrders;
        }
    }
}
