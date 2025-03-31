
namespace NanoFlow.ViewModels {

    public partial class FileExplorerWindowViewModel : ObservableObject {

        public event Action<string>? OnDetailsPanelSlide;

        public ObservableCollection<FolderItem> FolderItems { get; set; } = FolderService.GetFolders();

        public ObservableCollection<GcodeItem>? GcodeItems { get; set; } = [];

        public ObservableCollection<GcodeItem> FilteredGcodeItems { get; set; } = [];


        [ObservableProperty]
        string searchBoxText = string.Empty;

        [ObservableProperty]
        public FolderItem? selectedFolderItem;

        [ObservableProperty]
        public GcodeItem? selectedFile;

        [ObservableProperty]
        private Visibility searchBoxVisibility = Visibility.Collapsed;

        [RelayCommand]
        void FolderSeleted() {

            if(SelectedFolderItem is not null) {

                GcodeItems?.Clear();

                var newItems = FolderService.GetgcodeItems(SelectedFolderItem.FileName);

                if(newItems.Count > 0) {
                    foreach(var item in newItems) {
                        GcodeItems?.Add(item);
                    }

                    SearchBoxVisibility = Visibility.Visible;

                    FilteredGcodeItems.Clear();
                    foreach(var item in GcodeItems!) {
                        FilteredGcodeItems.Add(item);
                    }
                }
                else {
                    SearchBoxVisibility = Visibility.Collapsed;
                    FilteredGcodeItems.Clear(); // Clear filtered items if no files are found
                }
            }
            else {
                SearchBoxVisibility = Visibility.Collapsed;
                FilteredGcodeItems.Clear(); // Clear filtered items if no folder is selected
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

        partial void OnSelectedFileChanged(GcodeItem? value) {

            var fileName = value?.FileName ?? string.Empty;

            // Trigger the animation with the fileName
            OnDetailsPanelSlide?.Invoke(fileName);
        }

        partial void OnSearchBoxTextChanging(string value) {

            if(GcodeItems is not null) {
                FilteredGcodeItems.Clear();
                foreach(var item in GcodeItems) {
                    if(item.FileName.Contains(value, StringComparison.OrdinalIgnoreCase)) {
                        FilteredGcodeItems.Add(item);
                    }
                }
            }
        }
    }
}