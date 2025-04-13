namespace NanoFlow;

public partial class App : Application {

    public static ServiceProvider? Services { get; private set; }

    public App() {

        SyncfusionLicenseProvider.RegisterLicense(Constants.syncfusionLicenseKey);

        var services = new ServiceCollection();

        services.AddSingleton<MainViewModel>();

        services.AddTransient<FileExplorerWindowViewModel>();

        services.AddTransient<_3DViwerViewModel>();

        services.AddTransient<GcodeDialogViewModel>();

        services.AddTransient<IDialogService, DialogService>();

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

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Log.Information("Application started");


        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args) {

        var window = Services!.GetRequiredService<MainWindow>();
        window.SystemBackdrop = new MicaBackdrop();

        window.Maximize();

        WindowHelper.ConfigureCustomTitleBar(window, Constants.appTitle);

        window.Activate();
    }
}