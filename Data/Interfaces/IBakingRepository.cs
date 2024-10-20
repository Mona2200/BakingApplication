using BakingApplication.Data.Enitties;
using BakingApplication.Models;

namespace BakingApplication.Data.Interfaces;

public interface IBakingRepository
{
    IAsyncEnumerable<Baking> GetBakingsAsAsyncEnumerable(string? search = null);

    IAsyncEnumerable<Baking> GetBakingsByOrderAsAsyncEnumerable(int orderId);

    Task<bool> AnyBeaconByIdAsync(int id);

    Task<bool> AnyBeaconByNameAsync(string name);

    Task<Baking> AddBakingAsync(BakingModel baking);

    Task UpdateBakingAsync(BakingModel baking);

    Task DeleteBakingAsync(int id);
}
