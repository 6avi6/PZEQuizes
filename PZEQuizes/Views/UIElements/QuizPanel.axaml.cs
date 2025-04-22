using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PZEQuizes.ViewModels;

namespace PZEQuizes.Views.UIElements
{

    public partial class QuizPanel : UserControl
    {
        public QuizPanel()
        {
            InitializeComponent();
            DataContext = new QuizPanelViewModel();
        }
    }
}