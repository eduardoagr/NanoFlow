using Constants = NanoFlow.Helpers.Constants;

namespace NanoFlow.Services;

public static class FolderService {

    private static readonly Dictionary<string, string> FolderPaths = new() {
            { Constants.desktop, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) },
            { Constants.documents, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) },
            { Constants.downloads, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Constants.downloads) },
            { Constants.nanoFlowFolder, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Constants.nanoFlowFolder) }
        };

    public static ObservableCollection<FolderItem> GetFolders() {
        return [
                new FolderItem(Constants.desktop, Constants.GetImagePath(Constants.Folder), 0, DateTime.MinValue, FolderPaths[Constants.desktop]),
                new FolderItem(Constants.documents, Constants.GetImagePath(Constants.Folder), 0, DateTime.MinValue, FolderPaths[Constants.documents]),
                new FolderItem(Constants.downloads, Constants.GetImagePath(Constants.Folder), 0, DateTime.MinValue, FolderPaths[Constants.downloads]),
                new FolderItem(Constants.nanoFlowFolder, Constants.GetImagePath(Constants.Folder), 0, DateTime.MinValue, FolderPaths[Constants.nanoFlowFolder])
            ];
    }

    public static List<GcodeItem> GetgcodeItems(string folderName) {
        if(!FolderPaths.TryGetValue(folderName, out string folderPath)) {
            throw new ArgumentException($"Invalid folder name: {folderName}");
        }

        var gcodeItems = new List<GcodeItem>();
        var files = Directory.GetFiles(folderPath, $"*{Constants.gcode}");

        foreach(var file in files) {
            FileInfo fileInfo = new(file);
            gcodeItems.Add(new GcodeItem(
                Path.GetFileName(file),
                Constants.GetImagePath(Constants.gcodeFolder.ToLower()),
                fileInfo.Length,
                fileInfo.CreationTime,
                fileInfo.FullName
            ));
        }

        return gcodeItems;
    }
}
