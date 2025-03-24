using NanoFlow.Services;

namespace NanoFlow.ViewModels {
    public partial class FileExplorerWindowViewModel : ObservableObject {

        public ObservableCollection<FolderItem> FolderItems { get; set; } = FolderService.GetFolders();

        public ObservableCollection<GcodeItem>? GcodeItems { get; set; } = [];

        [ObservableProperty]
        public FolderItem? selectedFolderItem;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DetailsStackPanelVisibility))]
        public GcodeItem? selectedFile;

        public Visibility DetailsStackPanelVisibility => SelectedFile is null
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
        void FileSelected() {


        }
    }
}
