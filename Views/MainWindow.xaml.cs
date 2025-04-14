namespace NanoFlow.Views;

public sealed partial class MainWindow : WindowEx {

    public MainViewModel? ViewModel { get; set; }

    public MainWindow(MainViewModel mainViewModel) {

        InitializeComponent();

        ViewModel = mainViewModel;

        rootContainer.DataContext = ViewModel;

        mainViewModel.ComPortsListAction += async (comPorts) => {

            if(comPorts != null) {

                var portsListView = new ListView {

                    ItemsSource = comPorts,
                    SelectionMode = ListViewSelectionMode.Single,
                };

                var comDlg = new ContentDialog {
                    Title = "Select COM Port",
                    Content = portsListView,
                    PrimaryButtonText = Constants.ok,
                    CloseButtonText = Constants.cancel,
                    XamlRoot = Content.XamlRoot
                };

                if(await comDlg.ShowAsync() == ContentDialogResult.Primary) {

                    if(portsListView.SelectedItem is string selectedPort) {

                        Constants.printerPort = selectedPort;

                        Constants.OpenedPort = new SerialPort(selectedPort, Constants.printerBaudRate);

                        Constants.OpenedPort.Open();

                        ConnectedPrinterIndictor.Fill = new SolidColorBrush(Colors.Green);

                        PrinterStatusText.Text = "Ready";
                    }
                }
            }
        };

    }
}
