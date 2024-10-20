using BakingApplication.Data.Enitties;
using BakingApplication.Data.Interfaces;
using BakingApplication.Models;
using BakingApplication.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BakingApplication.Data.Repositories;

public class BakingRepository : IBakingRepository
{
    private readonly BakingApplicationContext _context;

    public BakingRepository(BakingApplicationContext context)
    {
        _context = context;
    }

    public async Task<Baking> AddBakingAsync(BakingModel bakingModel)
    {
        byte[]? picture = null;
        if (bakingModel.HasPicture)
            picture = BitmapToByteArrayConverter.Convert(bakingModel.Picture!);
        Baking baking = new()
        {
            Name = bakingModel.Name,
            Description = bakingModel.Description,
            Weight = bakingModel.Weight,
            Cost = bakingModel.Cost,
            Picture = picture,
            HasPicture = bakingModel.HasPicture
        };
        await _context.Bakings.AddAsync(baking);
        await _context.SaveChangesAsync();
        return baking;
    }

    public async Task<bool> AnyBeaconByIdAsync(int id)
    {
        bool result = await _context.Bakings.AnyAsync(b => b.Id == id);
        return result;
    }

    public async Task<bool> AnyBeaconByNameAsync(string name)
    {
        bool result = await _context.Bakings.AnyAsync(b => b.Name == name);
        return result;
    }

    public async Task DeleteBakingAsync(int id)
    {
        Baking? baking = await _context.Bakings.SingleOrDefaultAsync(b => b.Id == id);
        if (baking != null)
        {
            List<BakingOrder> bakingOrders = _context.BakingOrders.Where(bo => bo.BakingId == id).ToList();
            int[] bakingOrderIds = bakingOrders.Select(bo => bo.Id).ToArray();
            List<Order> orders = _context.Orders.Where(o => bakingOrderIds.Contains(o.Id)).ToList();
            _context.Bakings.Remove(baking);
            _context.BakingOrders.RemoveRange(bakingOrders);
            _context.Orders.RemoveRange(orders);
            await _context.SaveChangesAsync();
        }
    }

    public IAsyncEnumerable<Baking> GetBakingsAsAsyncEnumerable(string? search = null)
    {
        if (search != null)
            return _context.Bakings.Where(b => b.Name.ToLowerInvariant().Contains(search.ToLowerInvariant())).AsAsyncEnumerable();
        return _context.Bakings.AsAsyncEnumerable();
    }

    public async IAsyncEnumerable<Baking> GetBakingsByOrderAsAsyncEnumerable(int orderId)
    {
        await foreach (BakingOrder bakingOrder in _context.BakingOrders.Where(bo => bo.OrderId == orderId).AsAsyncEnumerable())
        {
            Baking baking = _context.Bakings.Find(bakingOrder.BakingId)!;
            yield return baking;
        }
    }

    public async Task UpdateBakingAsync(BakingModel bakingModel)
    {
        Baking? baking = await _context.Bakings.FirstOrDefaultAsync(b => b.Id == bakingModel.Id);
        if (baking == null)
            return;
        byte[]? picture = null;
        if (bakingModel.HasPicture)
            picture = BitmapToByteArrayConverter.Convert(bakingModel.Picture!);
        baking.Name = bakingModel.Name;
        baking.Description = bakingModel.Description;
        baking.Weight = bakingModel.Weight;
        baking.Cost = bakingModel.Cost;
        baking.Picture = picture;
        baking.HasPicture = bakingModel.HasPicture;
        _context.Bakings.Update(baking);
        await _context.SaveChangesAsync();
    }
}
