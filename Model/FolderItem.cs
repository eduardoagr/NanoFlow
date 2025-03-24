namespace NanoFlow.Model {

    public class FolderItem(string fileName, string iconPath, long fileSize, DateTime creationTime)
        : FileItem(fileName, iconPath, fileSize, creationTime) {
    }
}
