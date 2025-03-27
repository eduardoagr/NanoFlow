
namespace NanoFlow.ViewModels {

    public partial class FileExplorerWindowViewModel(IServiceProvider serviceProvider) : ObservableObject {

        public event Action<string>? OnDetailsPanelSlide;

        public ObservableCollection<FolderItem> FolderItems { get; set; } = FolderService.GetFolders();

        public ObservableCollection<GcodeItem>? GcodeItems { get; set; } = [];

        [ObservableProperty]
        public FolderItem? selectedFolderItem;

        [ObservableProperty]
        public GcodeItem? selectedFile;

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

        partial void OnSelectedFileChanged(GcodeItem? value) {

            var fileName = value?.FileName ?? string.Empty;
            Debug.WriteLine($"FileName changed to: {fileName}");
  
            // Trigger the animation with the fileName
            OnDetailsPanelSlide?.Invoke(fileName);
        }
    }
}