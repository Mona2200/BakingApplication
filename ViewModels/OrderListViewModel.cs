using BakingApplication.Commands;
using BakingApplication.Data.Enitties;
using BakingApplication.Data.Interfaces;
using BakingApplication.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace BakingApplication.ViewModels;

public class OrderListViewModel : INotifyPropertyChanged
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBakingRepository _bakingRepository;

    private ObservableCollection<OrderModel> _orderModels;
    public ObservableCollection<OrderModel> OrderModels
    {
        get => _orderModels;
        set
        {
            _orderModels = value;
            OnPropertyChanged(nameof(OrderModels));
        }
    }

    public BitmapImage NoPhotoBakingImage { get; private set; }

    public RelayCommand CloseWindowCommand { get; set; }
    public RelayCommand RepeatOrderCommand { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public OrderListViewModel(IOrderRepository orderRepository, IBakingRepository bakingRepository)
    {
        _orderRepository = orderRepository;
        _bakingRepository = bakingRepository;
        OrderModels = InitilizeOrderCollection();
        NoPhotoBakingImage = new BitmapImage();
        NoPhotoBakingImage.BeginInit();
        NoPhotoBakingImage.StreamSource = new FileStream("Images/No_photo.png", FileMode.Open, FileAccess.Read);
        NoPhotoBakingImage.EndInit();
    }

    public ObservableCollection<OrderModel> InitilizeOrderCollection()
    {
        ObservableCollection<OrderModel> orders = [];
        foreach (Order order in _orderRepository.GetOrders())
        {
            OrderModel orderModel = new()
            {
                Id = order.Id,
                Amount = order.Amount,
                Date = order.Date,
            };

            List<Baking> bakings = _bakingRepository.GetBakingsByOrder(order.Id);
            ObservableCollection<OrderBakingModel> orderBakingModels = [];

            foreach (Baking baking in bakings)
            {
                if (orderBakingModels.Any(ob => ob.BakingId == baking.Id))
                    orderBakingModels.First(ob => ob.BakingId == baking.Id).Count += 1;
                else
                {
                    BitmapImage? image = null;
                    if (baking.HasPicture)
                    {
                        image = new BitmapImage();
                        image.BeginInit();
                        image.StreamSource = new MemoryStream(baking.Picture!);
                        image.EndInit();
                    }
                    OrderBakingModel orderBakingModel = new(orderModel)
                    {
                        BakingId = baking.Id,
                        BakingModel = new BakingModel()
                        {
                            Id = baking.Id,
                            Name = baking.Name,
                            Description = baking.Description,
                            Weight = baking.Weight,
                            Cost = baking.Cost,
                            Picture = image ?? NoPhotoBakingImage
                        },
                        Count = 1
                    };
                    orderBakingModels.Add(orderBakingModel);
                }
            }
            orderModel.BakingCount = orderBakingModels;
            orders.Add(orderModel);
        }
        return new ObservableCollection<OrderModel>(orders.OrderByDescending(o => o.Date));
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
