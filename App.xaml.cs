using Constants = NanoFlow.Helpers.Constants;

namespace NanoFlow;

public partial class App : Application {

    public static ServiceProvider? Services { get; private set; }

    public App() {

        SyncfusionLicenseProvider.RegisterLicense(Constants.syncfusionLicenseKey);

        var services = new ServiceCollection();

        services.AddSingleton<MainViewModel>();

        services.AddTransient<FileExplorerWindowViewModel>();

        services.AddSingleton<NotificationHelper>();

        services.AddTransient<_3DViwerViewModel>();

        services.AddTransient<GcodeDialogViewModel>();

        services.AddTransient<StlSettingsDialogViewModel>();

        services.AddTransient<IDialogService, DialogService>();

        services.AddTransient(p =>
                            new StlSettingsDialog(p.GetRequiredService<StlSettingsDialogViewModel>()));

        services.AddTransient(p =>
                            new GcodeSettingsDialog(p.GetRequiredService<GcodeDialogViewModel>()));

        services.AddTransient(p =>
                             new MainWindow(
                                 p.GetRequiredService<MainViewModel>()));

        services.AddTransient(p =>
                             new FileExplorerWindow(
                                 p.GetRequiredService<FileExplorerWindowViewModel>()));

        services.AddTransient(p =>
                         new _3DViwerWidnow(
                             p.GetRequiredService<_3DViwerViewModel>()));


        Services = services.BuildServiceProvider();

        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args) {

        var window = Services!.GetRequiredService<MainWindow>();

        WindowHelper.ConfigureCustomTitleBar(window, Constants.appTitle);

        WindowHelper.MaximizeWindow(window);

        window.Activate();
    }
}