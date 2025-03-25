namespace NanoFlow.Model {

    public class FileItem(string fileName, string iconPath, long fileSize, DateTime createdTime, string filePath) {

        public string FileName { get; set; } = fileName;

        public string IconPath { get; set; } = iconPath;

        public long FileSize { get; set; } = fileSize;

        public DateTime CreationTime { get; set; } = createdTime;

        public string FilePath { get; set; } = filePath;


        //Helpers

        public string GetBaseFileName => Path.GetFileNameWithoutExtension(FileName);

        public string GetFileExtention => Path.GetExtension(FileName);

        public string GetFormattedFileSize {
            get {
                if(FileSize < 1024) {
                    return $"{FileSize} B"; // Files smaller than 1 KB are displayed in bytes.
                }
                else if(FileSize < 1024 * 1024) {
                    return $"{FileSize / 1024.0:F2} KB"; // Files between 1 KB and 1 MB are displayed in KB.
                }
                else {
                    return $"{FileSize / (1024.0 * 1024.0):F2} MB"; // Files larger than 1 MB are displayed in MB.
                }
            }
        }
    }
}
