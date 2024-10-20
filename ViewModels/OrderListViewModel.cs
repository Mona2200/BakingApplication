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

    private ObservableCollection<OrderModel> _orderModels = new();
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
    public RelayCommand DeleteOrderCommand { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public OrderListViewModel(IOrderRepository orderRepository, IBakingRepository bakingRepository)
    {
        _orderRepository = orderRepository;
        _bakingRepository = bakingRepository;
        NoPhotoBakingImage = new BitmapImage();
        NoPhotoBakingImage.BeginInit();
        NoPhotoBakingImage.StreamSource = new FileStream("Images/No_photo.png", FileMode.Open, FileAccess.Read);
        NoPhotoBakingImage.EndInit();
        InitilizeOrderCollection().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public async Task InitilizeOrderCollection()
    {
        await foreach (Order order in _orderRepository.GetOrdersAsAsyncEnumerable())
        {
            OrderModel orderModel = new()
            {
                Id = order.Id,
                Amount = order.Amount,
                Date = order.Date,
            };

            ObservableCollection<OrderBakingModel> orderBakingModels = [];
            await foreach (Baking baking in _bakingRepository.GetBakingsByOrderAsAsyncEnumerable(order.Id))
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
            OrderModels.Add(orderModel);
        }
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
