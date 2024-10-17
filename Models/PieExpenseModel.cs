using System.Windows.Media;

namespace BakingApplication.Models;

public class PieExpenseModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public double Amount { get; set; }

    public double Percent {  get; set; }

    public Color Color { get; set; }
}
