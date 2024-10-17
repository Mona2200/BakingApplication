using BakingApplication.Commands;
using BakingApplication.Data.Enitties;
using BakingApplication.Data.Interfaces;
using BakingApplication.Models;
using BakingApplication.Utilities;
using BakingApplication.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace BakingApplication.ViewModels;

public class CatalogViewModel : INotifyPropertyChanged
{
    private readonly MainWindow _mainWindow;
    private readonly IBakingRepository _bakingRepository;
    private readonly OrderModel _orderModel;
    private readonly AddBakingViewModel _addBakingViewModel;
    private AddBakingWindow _addBakingWindow;
    private RemoveBakingWindow _removeBakingWindow;
    private readonly RemoveBakingViewModel _removeBakingViewModel;

    public BitmapImage NoPhotoBakingImage { get; private set; }

    private ObservableCollection<BakingModel> _bakings;
    public ObservableCollection<BakingModel> Bakings
    {
        get => _bakings;
        set
        {
            _bakings = value;
            OnPropertyChanged(nameof(Bakings));
        }
    }

    public RelayCommand AddBakingToOrderCommand { get; set; }
    public RelayCommand AddBakingWindowCommand { get; set; }
    public RelayCommand EditBakingWindowCommand { get; set; }
    public RelayCommand RemoveBakingWindowCommand { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public CatalogViewModel(
        IBakingRepository bakingRepository,
        MainWindow mainWindow,
        AddBakingViewModel addBakingViewModel,
        RemoveBakingViewModel removeBakingViewModel,
        OrderModel orderModel)
    {
        _bakingRepository = bakingRepository;
        _mainWindow = mainWindow;
        _orderModel = orderModel;
        _addBakingViewModel = addBakingViewModel;
        _removeBakingViewModel = removeBakingViewModel;

        NoPhotoBakingImage = new BitmapImage();
        NoPhotoBakingImage.BeginInit();
        NoPhotoBakingImage.StreamSource = new FileStream("Images/No_photo.png", FileMode.Open, FileAccess.Read);
        NoPhotoBakingImage.EndInit();
        Bakings = InitilizeBakingCollection();
        AddBakingWindowCommand = new(o => ShowAddBakingWindows(o));
        EditBakingWindowCommand = new(o => ShowEditBakingWindow(o));
        RemoveBakingWindowCommand = new(o => ShowRemoveBakingWindow(o));
        AddBakingToOrderCommand = new(o => AddBakingToOrder(o));
    }

    private void AddBakingToOrder(object o)
    {
        if (_orderModel.BakingCount.Count == 10)
            return;

        BakingModel bakingModel = (BakingModel)o;
        if (_orderModel.BakingCount.Any(b => b.BakingId == bakingModel.Id))
            _orderModel.BakingCount.Single(b => b.BakingId == bakingModel.Id).Count += 1;
        else
            _orderModel.BakingCount.Add(new OrderBakingModel(_orderModel)
            {
                BakingId = bakingModel.Id,
                BakingModel = bakingModel,
                Count = 1
            });
    }

    private void ShowRemoveBakingWindow(object obj)
    {
        _removeBakingViewModel.CloseWindowCommand = new(o => _removeBakingWindow.DialogResult = false);
        _removeBakingViewModel.RemoveBakingCommand = new(async o => await RemoveBakingModel(o));
        _removeBakingViewModel.CloseWindowCommand = new(o => _removeBakingWindow.DialogResult = false);
        _removeBakingViewModel.SelectedBaking = (BakingModel)obj;
        _removeBakingWindow = new();
        _removeBakingWindow.Owner = _mainWindow;
        _removeBakingWindow.DataContext = _removeBakingViewModel;
        _removeBakingWindow.ShowDialog();
    }

    private async Task RemoveBakingModel(object obj)
    {
        BakingModel selectedBaking = (BakingModel)obj;
        await _bakingRepository.DeleteBakingAsync(selectedBaking.Id);
        Bakings.Remove(selectedBaking);
        OrderBakingModel? selectedBakingFromDict = _orderModel.BakingCount.FirstOrDefault(b => b.BakingId == selectedBaking.Id);
        if (selectedBakingFromDict != null)
            _orderModel.BakingCount.Remove(selectedBakingFromDict);
        _removeBakingViewModel.CloseWindowCommand.Execute(obj);
    }

    private void ShowEditBakingWindow(object obj)
    {
        _addBakingViewModel.CloseWindowCommand = new(o => _addBakingWindow.DialogResult = false);
        _addBakingViewModel.AddBakingCommand = new(async o => await EditBakingModel(o));
        _addBakingViewModel.AddBaking = (BakingModel)obj;
        _addBakingWindow = new();
        _addBakingWindow.Owner = _mainWindow;
        _addBakingWindow.DataContext = _addBakingViewModel;
        _addBakingWindow.ShowDialog();
    }

    public ObservableCollection<BakingModel> InitilizeBakingCollection()
    {
        List<BakingModel> bakings = [];
        foreach (Baking baking in _bakingRepository.GetBakings())
        {
            BitmapImage? image = null;
            if (baking.HasPicture)
            {
                image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = new MemoryStream(baking.Picture!);
                image.EndInit();
            }
            bakings.Add(new BakingModel()
            {
                Id = baking.Id,
                Name = baking.Name,
                Description = baking.Description,
                Weight = baking.Weight,
                Cost = baking.Cost,
                Picture = image ?? NoPhotoBakingImage
            });
        }
        return new ObservableCollection<BakingModel>(bakings);
    }

    public void ShowAddBakingWindows(object obj)
    {
        _addBakingViewModel.CloseWindowCommand = new(o => _addBakingWindow.DialogResult = false);
        _addBakingViewModel.AddBakingCommand = new(async o => await AddBakingModel(o));
        _addBakingViewModel.AddBaking = new();
        _addBakingViewModel.AddBaking.Picture = NoPhotoBakingImage;
        _addBakingViewModel.AddBaking.HasPicture = false;
        _addBakingWindow = new();
        _addBakingWindow.Owner = _mainWindow;
        _addBakingWindow.DataContext = _addBakingViewModel;
        _addBakingWindow.ShowDialog();
    }

    private async Task AddBakingModel(object obj)
    {
        BakingModel addBaking = (BakingModel)obj;
        if (string.IsNullOrEmpty(addBaking.Name))
        {
            _addBakingViewModel.ErrorMessage = "Название не введено";
            return;
        }
        if (addBaking.Weight == 0)
        {
            _addBakingViewModel.ErrorMessage = "Вес не указан";
            return;
        }
        Baking bakingResult = await _bakingRepository.AddBakingAsync(addBaking);
        addBaking.Id = bakingResult.Id;
        Bakings.Add(addBaking);
        _addBakingViewModel.CloseWindowCommand.Execute(obj);
    }

    private async Task EditBakingModel(object obj)
    {
        BakingModel addBaking = (BakingModel)obj;
        if (string.IsNullOrEmpty(addBaking.Name))
        {
            _addBakingViewModel.ErrorMessage = "Название не введено";
            return;
        }
        if (addBaking.Weight == 0)
        {
            _addBakingViewModel.ErrorMessage = "Вес не указан";
            return;
        }
        await _bakingRepository.UpdateBakingAsync(addBaking);        
        _addBakingViewModel.CloseWindowCommand.Execute(obj);
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
