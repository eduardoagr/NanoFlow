namespace NanoFlow.Helpers;
public static class FilePathManager {

    private static readonly string DesignsFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        Constants.nanoFlowFolder);

    static FilePathManager() {
        // Ensure the Designs folder exists
        if(!Directory.Exists(DesignsFolder)) {
            Directory.CreateDirectory(DesignsFolder);
        }
    }

    public static string GetFilePath(string fileName) {
        return Path.Combine(DesignsFolder, fileName);
    }
}
