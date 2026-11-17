using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GameLibrary.Data;
using GameLibrary.Helpers;
using GameLibrary.Models;

namespace GameLibrary;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    public static AppDbContext DbContext { get; } = new AppDbContext();
    public static User? CurrentUser {  get; set; }
    public override void OnFrameworkInitializationCompleted()
    {
        ThemeHelper.Load();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
