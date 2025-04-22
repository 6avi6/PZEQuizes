using Avalonia.Controls;
using Avalonia.Interactivity;
using PZEQuizes.ViewModels;
using Avalonia.Platform.Storage;

namespace PZEQuizes
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(this.StorageProvider, this);
        }
    }
}
