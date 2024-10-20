using BakingApplication.Data.Enitties;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BakingApplication.Models;

public class OrderModel : INotifyPropertyChanged
{
    public int Id { get; set; }

    private DateTime _data = DateTime.Now;
    public DateTime Date
    {
        get => _data;
        set
        {
            _data = value;
            OnPropertyChanged(nameof(Date));
        }
    }

    private int _amount;
    public int Amount
    {
        get => _amount;
        set
        {
            _amount = value;
            OnPropertyChanged(nameof(Amount));
        }
    }

    private ObservableCollection<OrderBakingModel> _bakingCount = new();
    public ObservableCollection<OrderBakingModel> BakingCount
    {
        get => _bakingCount;
        set
        {
            _bakingCount = value;
            OnPropertyChanged(nameof(_bakingCount));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
