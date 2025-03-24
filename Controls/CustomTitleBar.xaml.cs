namespace NanoFlow.Controls;

public sealed partial class CustomTitleBar : UserControl {

    public CustomTitleBarViewModel ViewModel { get; set; }

    public CustomTitleBar(Window window, CustomTitleBarViewModel viewModel) {

        InitializeComponent();

        ViewModel = viewModel ?? new CustomTitleBarViewModel(window);

        DataContext = ViewModel;
    }


}
