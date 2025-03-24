namespace NanoFlow.Model {

    public class GcodeItem(string fileName, string iconPath, long fileSize, DateTime creationTime)
        : FileItem(fileName, iconPath, fileSize, creationTime) { }
}
