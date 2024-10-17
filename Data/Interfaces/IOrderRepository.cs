using BakingApplication.Data.Enitties;

namespace BakingApplication.Data.Interfaces;

public interface IOrderRepository
{
    IAsyncEnumerable<Order> GetOrdersAsAsyncEnumerable();

    Task<List<Order>> GetOrdersAsync();

    Task<List<Order>> GetOrdersByDateAsync(DateOnly date);

    List<Order> GetOrders();

    Task<bool> AnyOrderByIdAsync(int id);

    Task<Order> AddOrderAsync(Order order, Dictionary<int, int> bakingIdCounts);

    Task DeleteOrderAsync(int id);
}
