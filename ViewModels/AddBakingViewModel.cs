using BakingApplication.Commands;
using BakingApplication.Data.Interfaces;
using BakingApplication.Models;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace BakingApplication.ViewModels;

public class AddBakingViewModel : INotifyPropertyChanged
{
    private readonly IBakingRepository _bakingRepository;
    public BitmapImage NoPhotoBakingImage { get; private set; }
    public BakingModel AddBaking { get; set; }


    private string _errorMessage;
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged(nameof(ErrorMessage));
        }
    }

    public RelayCommand AddBakingImageCommand { get; set; }
    public RelayCommand RemoveBakingImageCommand { get; set; }
    public RelayCommand CloseWindowCommand { get; set; }
    public RelayCommand AddBakingCommand { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public AddBakingViewModel(IBakingRepository bakingRepository)
    {
        _bakingRepository = bakingRepository;
        NoPhotoBakingImage = new BitmapImage();
        NoPhotoBakingImage.BeginInit();
        NoPhotoBakingImage.StreamSource = new FileStream("Images/No_photo.png", FileMode.Open, FileAccess.Read);
        NoPhotoBakingImage.EndInit();
        AddBaking = new();
        AddBakingImageCommand = new(o => OpenFileWindowToLoadBakingImage());
        RemoveBakingImageCommand = new(o => RemoveBakingImage());
        if (!AddBaking.HasPicture)
            RemoveBakingImage();
    }

    private void RemoveBakingImage()
    {
        AddBaking.Picture = NoPhotoBakingImage;
        AddBaking.HasPicture = false;
    }

    private void OpenFileWindowToLoadBakingImage()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
        if (openFileDialog.ShowDialog() == true)
        {
            AddBaking.Picture = new BitmapImage();
            AddBaking.Picture.BeginInit();
            AddBaking.Picture.UriSource = new Uri(openFileDialog.FileName, UriKind.Absolute);
            AddBaking.Picture.EndInit();
            AddBaking.HasPicture = true;
        }
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
