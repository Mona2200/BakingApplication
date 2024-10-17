using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BakingApplication.Data.Enitties;

public class Baking
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public int Cost { get; set; }

    public int Weight { get; set; }

    public bool HasPicture { get; set; }

    public byte[]? Picture { get; set; }

    public ICollection<Order> OrdersTable { get; set; }
}
