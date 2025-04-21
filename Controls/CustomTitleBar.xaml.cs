namespace NanoFlow.Controls;

public sealed partial class CustomTitleBar : UserControl {

    public CustomTitleBarViewModel ViewModel { get; set; }

    public CustomTitleBar(CustomTitleBarViewModel viewModel) {

        InitializeComponent();

        ViewModel = viewModel ?? new CustomTitleBarViewModel();

        DataContext = ViewModel;
    }


}
