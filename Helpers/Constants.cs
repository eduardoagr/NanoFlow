namespace NanoFlow.Helpers;

public class Constants {

    public const string syncfusionLicenseKey = "Mzc4MTU5N0AzMjM5MmUzMDJlMzAzYjMyMzkzYkJYdFJEcnlId2RHNm9seVhJWEFKUkppMDltSzgrOEJRYzdrcDdLZVJGMXM9";

    public const string nanoFlowFolder = "NanoFlow";

    public const string appTitle = "NanoFlow";

    public const int gridSpacing = 20;

    public const int _canvasSize = 378;

    public const string explorer = "explorer.exe";

    public const string gcode = ".gcode";

    public const string stl = ".stl";

    public const string ok = "OK";

    public const string cancel = "Cancel";

    public const string save = "Save";

    public const string saveDesign = "Save Design";

    public const string pointsSeletion = "Select point to draw";

    public const string baseImagePath = "/Assets";


    // Folders
    public const string desktop = "Desktop";
    public const string documents = "Documents";
    public const string downloads = "Downloads";
    public const string gcodeFolder = "Gcode";

    public const string Folder = "folder";


    public static string GetImagePath(string name) {

        return $"{baseImagePath}/{name}.png";
    }

}
