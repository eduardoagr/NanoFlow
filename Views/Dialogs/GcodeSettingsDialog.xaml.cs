namespace NanoFlow.Views.Dialogs;

public sealed partial class GcodeSettingsDialog : ContentDialog {

    public GcodeDialogViewModel _viewModel { get; set; }

    public GcodeSettingsDialog(GcodeDialogViewModel viewModel) {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
    }
}
