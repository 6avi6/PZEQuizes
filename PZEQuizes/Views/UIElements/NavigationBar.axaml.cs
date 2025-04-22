using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PZEQuizes.ViewModels.UIElements;

namespace PZEQuizes.Views.UIElements
{
    public partial class NavigationBar : UserControl
    {
        public NavigationBar()
        {
            InitializeComponent();
            DataContext = new NavigationBarViewModel();
        }
    }
}