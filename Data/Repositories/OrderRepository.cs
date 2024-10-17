using BakingApplication.Data.Enitties;
using BakingApplication.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BakingApplication.Data.Repositories;

public class OrderRepository(BakingApplicationContext context) : IOrderRepository
{
    public async Task<Order> AddOrderAsync(Order order, Dictionary<int, int> bakingIdCounts)
    {
        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();
        List<BakingOrder> bakingOrders = [];
        foreach (KeyValuePair<int, int> bakingIdCount in bakingIdCounts)
            for (int i = 0; i < bakingIdCount.Value; i++)
                bakingOrders.Add(new BakingOrder()
                {
                    BakingId = bakingIdCount.Key,
                    OrderId = order.Id
                });
        await context.BakingOrders.AddRangeAsync(bakingOrders);
        await context.SaveChangesAsync();
        return order;
    }

    public async Task<bool> AnyOrderByIdAsync(int id)
    {
        bool result = await context.Orders.AnyAsync(b => b.Id == id);
        return result;
    }

    public async Task DeleteOrderAsync(int id)
    {
        Order? order = await context.Orders.SingleOrDefaultAsync(b => b.Id == id);
        if (order != null)
        {
            context.Orders.Remove(order);
            await context.SaveChangesAsync();
        }
    }

    public List<Order> GetOrders()
    {
        return context.Orders.ToList();
    }

    public IAsyncEnumerable<Order> GetOrdersAsAsyncEnumerable()
    {
        return context.Orders.AsAsyncEnumerable();
    }

    public Task<List<Order>> GetOrdersAsync()
    {
        return context.Orders.ToListAsync();
    }

    public Task<List<Order>> GetOrdersByDateAsync(DateOnly date)
    {
        return context.Orders.Where(o => o.Date.Day == date.Day && o.Date.Month == date.Month && o.Date.Year == date.Year).ToListAsync();
    }
}
