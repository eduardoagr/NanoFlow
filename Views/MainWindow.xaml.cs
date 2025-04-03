namespace NanoFlow.Views;

public sealed partial class MainWindow : WindowEx {

    public MainViewModel? ViewModel { get; set; }

    public MainWindow(MainViewModel mainViewModel) {

        InitializeComponent();

        ViewModel = mainViewModel;

        rootContainer.DataContext = ViewModel;

    }
}
