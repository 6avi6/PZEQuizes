using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using System.IO;
using PZEQuizes;
using System;

namespace PZEQuizes;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow();
            var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "logo.ico");
            mainWindow.Icon = new WindowIcon(new Bitmap(iconPath));

            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}