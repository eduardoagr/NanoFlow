namespace NanoFlow.Views;

public sealed partial class FileExplorerWindow : Window {

    public FileExplorerWindowViewModel FileExplorerWindowViewModel { get; set; }

    public FileExplorerWindow(FileExplorerWindowViewModel explorerWindowViewModel) {

        InitializeComponent();

        FileExplorerWindowViewModel = explorerWindowViewModel;

        rootContainer.DataContext = FileExplorerWindowViewModel;
    }
}
