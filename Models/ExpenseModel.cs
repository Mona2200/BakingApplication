using BakingApplication.Data.Enitties;
using System.ComponentModel;
using System.Xml.Linq;

namespace BakingApplication.Models;

public class ExpenseModel : INotifyPropertyChanged
{
    private ExpenseTypeModel _expenseType = new();
    public ExpenseTypeModel ExpenseType
    {
        get => _expenseType;
        set
        {
            _expenseType = value;
            OnPropertyChanged(nameof(ExpenseType));
        }
    }

    public int Id { get; set; }

    public DateTime Time { get; set; } = DateTime.Now;

    public int Amount { get; set; }

    public string Description { get; set; } = string.Empty;


    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
