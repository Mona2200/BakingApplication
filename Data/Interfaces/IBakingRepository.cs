using BakingApplication.Data.Enitties;
using BakingApplication.Models;
using System.Collections.ObjectModel;

namespace BakingApplication.Data.Interfaces;

public interface IBakingRepository
{
    IAsyncEnumerable<Baking> GetBakingsAsAsyncEnumerable();

    Task<List<Baking>> GetBakingsAsync();

    List<Baking> GetBakings();

    List<Baking> GetBakingsByOrder(int orderId);

    Task<bool> AnyBeaconByIdAsync(int id);

    Task<bool> AnyBeaconByNameAsync(string name);

    Task<Baking> AddBakingAsync(BakingModel baking);

    Task UpdateBakingAsync(BakingModel baking);

    Task DeleteBakingAsync(int id);
}
