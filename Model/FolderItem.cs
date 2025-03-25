namespace NanoFlow.Model {

    public class FolderItem(string fileName, string iconPath, long fileSize, DateTime creationTime, string filePath)
        : FileItem(fileName, iconPath, fileSize, creationTime, filePath) {
    }
}
