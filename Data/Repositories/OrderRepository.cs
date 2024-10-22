using BakingApplication.Data.Enitties;
using BakingApplication.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BakingApplication.Data.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly BakingApplicationContext _context;

    public OrderRepository(BakingApplicationContext context)
    {
        _context = context;
        ClearOldData().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    private async Task ClearOldData()
    {
        DateTime timeMin = DateTime.Now - TimeSpan.FromDays(365);
        List<Order> orders = await _context.Orders.Where(o => o.Date < timeMin).ToListAsync();
        foreach (Order order in orders)
        {
            List<BakingOrder> bakingOrders = await _context.BakingOrders.Where(bo => bo.OrderId == order.Id).ToListAsync();
            _context.BakingOrders.RemoveRange(bakingOrders);
        }
        _context.Orders.RemoveRange(orders);
        await _context.SaveChangesAsync();
    }

    public async Task<Order> AddOrderAsync(Order order, Dictionary<int, int> bakingIdCounts)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
        List<BakingOrder> bakingOrders = [];
        foreach (KeyValuePair<int, int> bakingIdCount in bakingIdCounts)
            for (int i = 0; i < bakingIdCount.Value; i++)
                bakingOrders.Add(new BakingOrder()
                {
                    BakingId = bakingIdCount.Key,
                    OrderId = order.Id
                });
        await _context.BakingOrders.AddRangeAsync(bakingOrders);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<bool> AnyOrderByIdAsync(int id)
    {
        bool result = await _context.Orders.AnyAsync(b => b.Id == id);
        return result;
    }

    public async Task DeleteOrderAsync(int id)
    {
        Order? order = await _context.Orders.SingleOrDefaultAsync(b => b.Id == id);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }

    public IAsyncEnumerable<Order> GetOrdersAsAsyncEnumerable()
    {
        return _context.Orders.OrderByDescending(o => o.Date).AsAsyncEnumerable();
    }

    public IAsyncEnumerable<Order> GetOrdersByDateAsAsyncEnumerable(DateTime startTime, DateTime endTime)
    {
        return _context.Orders.Where(o => o.Date >= startTime.Date && o.Date <= endTime.Date).AsAsyncEnumerable();
    }
}
