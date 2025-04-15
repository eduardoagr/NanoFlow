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
                        var availablePorts = SerialPort.GetPortNames();

                        foreach(var port in availablePorts) {
                            if(selectedPort.Contains(port)) {
                                Constants.printerPort = port;
                                Constants.OpenedPort = new SerialPort(port, Constants.printerBaudRate);
                                Constants.OpenedPort.Open();

                                mainViewModel.IsComPortConnected = true;

                                if(mainViewModel.IsComPortConnected) {
                                    mainViewModel.ComPortStatus = Constants.connectedPrinter;
                                    ConnectedPrinterIndictor.Fill = new SolidColorBrush(Colors.Green);
                                }
                                else {
                                    mainViewModel.ComPortStatus = Constants.discconnectedPrinter;
                                    ConnectedPrinterIndictor.Fill = new SolidColorBrush(Colors.Red);
                                }
                            }
                        }
                    }
                }
            }
        }; // Close the ComPortsListAction lambda
    }
}