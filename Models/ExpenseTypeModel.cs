using BakingApplication.Data.Enitties;
using System.ComponentModel;

namespace BakingApplication.Models;

public class ExpenseTypeModel : INotifyPropertyChanged
{
    public int Id { get; set; }

    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ExpenseTypeModel()
    { }

    public ExpenseTypeModel(ExpenseType expenseType)
    { 
        Id = expenseType.Id;
        Name = expenseType.Name;
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
