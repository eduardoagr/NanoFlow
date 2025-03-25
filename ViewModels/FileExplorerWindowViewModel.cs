namespace NanoFlow.ViewModels {

    public partial class FileExplorerWindowViewModel(IServiceProvider serviceProvider) : ObservableObject {

        public ObservableCollection<FolderItem> FolderItems { get; set; } = FolderService.GetFolders();

        public ObservableCollection<GcodeItem>? GcodeItems { get; set; } = [];

        [ObservableProperty]
        public FolderItem? selectedFolderItem;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DetailsPanelVisibility))]
        public GcodeItem? selectedFile;

        public Visibility DetailsPanelVisibility => SelectedFile is null
            ? Visibility.Collapsed
            : Visibility.Visible;

        [RelayCommand]
        void FolderSeleted() {

            if(SelectedFolderItem is null) {
                return;
            }

            GcodeItems?.Clear();

            var newItems = FolderService.GetgcodeItems(SelectedFolderItem.FileName);

            if(newItems.Count > 0) {

                foreach(var item in newItems) {
                    GcodeItems?.Add(item);
                }

            }
        }

        [RelayCommand]
        void StartSimulation() {

            if(SelectedFile is not null) {

                // Create the ViewModel with SelectedFile
                var viewerViewModel = new _3DViwerViewModel(SelectedFile!);

                // Create the new window and pass the ViewModel to its constructor
                var newWindow = new _3DViwerWidnow(viewerViewModel);

                WindowHelper.CreateNewWindow(newWindow, SelectedFile!.FilePath);
            }
        }
    }
}