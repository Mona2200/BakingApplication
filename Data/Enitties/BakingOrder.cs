using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BakingApplication.Data.Enitties;

public class BakingOrder
{
    [Key]
    public int Id { get; set; }

    public int BakingId { get; set; }

    public int OrderId { get; set; }

    [ForeignKey(nameof(BakingId))]
    public Baking BakingTable { get; set; }

    [ForeignKey(nameof(OrderId))]
    public Order OrderTable { get; set; }
}
