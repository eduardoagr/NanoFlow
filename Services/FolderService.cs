using Constants = NanoFlow.Helpers.Constants;

namespace NanoFlow.Services {

    public class FolderService {

        public static ObservableCollection<FolderItem> GetFolders() {

            return [

                 new FolderItem(Constants.desktop,
                 Constants.GetImagePath(Constants.Folder),
                 0, DateTime.MinValue, string.Empty),

                 new FolderItem(Constants.documents, Constants.GetImagePath(Constants.Folder),
                 0, DateTime.MinValue, string.Empty),

                 new FolderItem(Constants.downloads, Constants.GetImagePath(Constants.Folder),
                 0, DateTime.MinValue, string.Empty),

                  new FolderItem(Constants.nanoFlowFolder, Constants.GetImagePath(Constants.Folder),
                 0, DateTime.MinValue, string.Empty),
            ];
        }

        public static List<GcodeItem> GetgcodeItems(string folderName) {

            var gcodeItems = new List<GcodeItem>();

            string folderPath = folderName switch {
                Constants.desktop => Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Constants.documents => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Constants.downloads => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Constants.downloads),
                Constants.nanoFlowFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Constants.nanoFlowFolder),
                _ => throw new ArgumentException("Invalid folder name")
            };


            var files = Directory.GetFiles(folderPath, $"*{Constants.gcode}");

            foreach(var file in files) {

                FileInfo fileInfo = new(file);

                gcodeItems.Add(new GcodeItem(Path.GetFileName(file),
                    Constants.GetImagePath(Constants.gcodeFolder.ToLower()),
                    fileInfo.Length, fileInfo.CreationTime, Path.GetFullPath(file)));
            }

            return gcodeItems;
        }

    }
}
