using System.ComponentModel.DataAnnotations;

namespace BakingApplication.Data.Enitties;

public class Order
{
    [Key]
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public int Amount { get; set; }

    public ICollection<Baking> BakingsTable { get; set; }
}
