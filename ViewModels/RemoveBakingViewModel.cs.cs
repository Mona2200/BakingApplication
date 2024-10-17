using BakingApplication.Commands;
using BakingApplication.Data.Enitties;
using BakingApplication.Data.Interfaces;
using BakingApplication.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace BakingApplication.ViewModels;

public class RemoveBakingViewModel : INotifyPropertyChanged
{
    public BakingModel SelectedBaking { get; set; }

    public RelayCommand RemoveBakingCommand { get; set; }
    public RelayCommand CloseWindowCommand { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public RemoveBakingViewModel()
    {
    }
}
