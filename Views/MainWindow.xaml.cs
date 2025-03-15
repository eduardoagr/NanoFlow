namespace NanoFlow.Views {

    public sealed partial class MainWindow : Window {

        public MainViewModel? ViewModel { get; set; }

        public MainWindow(MainViewModel mainViewModel) {

            InitializeComponent();

            ViewModel = mainViewModel;

            SetRootContainer.DataContext = ViewModel;

        }
    }
}
