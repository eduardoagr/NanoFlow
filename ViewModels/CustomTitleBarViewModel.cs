namespace NanoFlow.ViewModels;

public partial class CustomTitleBarViewModel(Window window) : ObservableObject {

    [ObservableProperty]
    public string appTitle = "Default Title";
}
