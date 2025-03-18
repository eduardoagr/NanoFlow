
namespace NanoFlow {

    public partial class App : Application {

        public static ServiceProvider? Services { get; private set; }

        public App() {

            SyncfusionLicenseProvider.RegisterLicense(Constants.SyncfusionLicenseKey);

            var services = new ServiceCollection();

            services.AddSingleton<MainViewModel>();

            services.AddSingleton<NotificationHelper>();

            services.AddSingleton<GcodeDialogViewModel>();

            services.AddTransient(p =>
                                new GcodeSettingsDialog(p.GetRequiredService<GcodeDialogViewModel>()));

            services.AddTransient(p =>
                                 new MainWindow(
                                     p.GetRequiredService<MainViewModel>()));

            Services = services.BuildServiceProvider();

            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args) {

            var window = Services!.GetRequiredService<MainWindow>();
            window.Activate();

            var hwnd = WindowNative.GetWindowHandle(window);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            var presenter = appWindow.Presenter as OverlappedPresenter;
            presenter?.Maximize();
        }
    }
}