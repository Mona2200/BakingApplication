﻿using BakingApplication.Data.Enitties;

namespace BakingApplication.Data.Interfaces;

public interface IOrderRepository
{
    IAsyncEnumerable<Order> GetOrdersAsAsyncEnumerable();

    IAsyncEnumerable<Order> GetOrdersByDateAsAsyncEnumerable(DateTime startTime, DateTime endTime);

    Task<bool> AnyOrderByIdAsync(int id);

    Task<Order> AddOrderAsync(Order order, Dictionary<int, int> bakingIdCounts);

    Task DeleteOrderAsync(int id);
}
