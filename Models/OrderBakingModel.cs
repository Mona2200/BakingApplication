using System.ComponentModel;

namespace BakingApplication.Models;

public class OrderBakingModel : INotifyPropertyChanged
{
    private OrderModel _orderModel;

    private int _bakingId;
    public int BakingId
    {
        get => _bakingId;
        set
        {
            _bakingId = value;
            OnPropertyChanged(nameof(BakingId));
        }
    }

    private BakingModel _bakingModel;
    public BakingModel BakingModel
    {
        get => _bakingModel;
        set
        {
            _bakingModel = value;
            OnPropertyChanged(nameof(BakingModel));
        }
    }

    private int _count;
    public int Count
    {
        get => _count;
        set
        {
            _orderModel.Amount -= _count * _bakingModel.Cost;
            _count = value;
            _orderModel.Amount += _count * _bakingModel.Cost;
            OnPropertyChanged(nameof(Count));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public OrderBakingModel(OrderModel orderModel)
    {
        _orderModel = orderModel;
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
