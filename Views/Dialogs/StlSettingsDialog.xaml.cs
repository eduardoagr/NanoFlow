namespace NanoFlow.Views.Dialogs;

public sealed partial class StlSettingsDialog : ContentDialog {

    public StlSettingsDialogViewModel _viewModel { get; set; }

    public StlSettingsDialog(StlSettingsDialogViewModel stlSettingsDialogViewModel) {
        InitializeComponent();

        _viewModel = stlSettingsDialogViewModel;

        DataContext = _viewModel;


    }
}
