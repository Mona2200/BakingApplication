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

        DateTime timeMin = DateTime.Now - TimeSpan.FromDays(90);
        List<Order> orders = _context.Orders.Where(o => o.Date < timeMin).ToList();
        _context.Orders.RemoveRange(orders);
        _context.SaveChanges();
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
            _context.Bakings.Remove(baking);
            await _context.SaveChangesAsync();
        }
    }

    public List<Baking> GetBakings()
    {
        return _context.Bakings.ToList();
    }

    public IAsyncEnumerable<Baking> GetBakingsAsAsyncEnumerable()
    {
        return _context.Bakings.AsAsyncEnumerable();
    }

    public Task<List<Baking>> GetBakingsAsync()
    {
        return _context.Bakings.ToListAsync();
    }

    public List<Baking> GetBakingsByOrder(int orderId)
    {
        List<Baking> bakings = [];
        IEnumerable<BakingOrder> bakingOrders = _context.BakingOrders.Where(bo => bo.OrderId == orderId);
        foreach (BakingOrder bakingOrder in bakingOrders)
        {
            Baking baking = _context.Bakings.Find(bakingOrder.BakingId)!;
            bakings.Add(baking);
        }
        return bakings;
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
