

using Serilog;



namespace NanoFlow.ViewModels {
    public partial class _3DViwerViewModel : ObservableObject {

        [ObservableProperty]
        private string fileContent; // G-code text content for display

        [ObservableProperty]
        private int fieldOfView = 160; // Camera Field of View

        [ObservableProperty]
        public PerspectiveCamera camera; // 3D Camera

        public DefaultEffectsManager EffectsManager { get; } = new DefaultEffectsManager(); // Effects Manager

        public LineGeometry3D Root { get; private set; } // Main geometry to render

        public Vector3 ModelCentroid { get; } = new Vector3(0, 0, 0); // Central rotation point

        // Constructor
        public _3DViwerViewModel(GcodeItem gcodeContent) {
            try {
                Log.Information("Initializing _3DViwerViewModel...");

                // Load G-code content
                if(gcodeContent != null && !string.IsNullOrWhiteSpace(gcodeContent.FilePath)) {
                    fileContent = !File.Exists(gcodeContent.FilePath)
                        ? string.Empty
                        : File.ReadAllText(gcodeContent.FilePath);

                    Log.Information("G-code file content loaded successfully for: {FilePath}", gcodeContent.FilePath);
                }
                else {
                    fileContent = string.Empty;
                    Log.Warning("G-code file path is invalid or not provided.");
                }

                // Initialize geometry
                if(!string.IsNullOrWhiteSpace(fileContent)) {
                    Root = GenerateGcodeGeometry(fileContent);
                    if(Root == null) {
                        Log.Error("Failed to generate geometry. Root is null.");
                    }
                    else {
                        Log.Information("Geometry successfully generated.");
                    }
                }
                else {
                    Root = new LineGeometry3D();
                    Log.Warning("FileContent is empty. Initialized with empty geometry.");
                }

                // Initialize the camera
                camera = new PerspectiveCamera {
                    Position = new Vector3(0, 0, 100), // Default position
                    LookDirection = new Vector3(0, 0, -100), // Default look direction
                    UpDirection = new Vector3(0, 1, 0), // Up vector
                    FieldOfView = fieldOfView // FOV from property
                };

                if(camera == null) {
                    Log.Error("Camera initialization failed. Camera is null.");
                }
                else {
                    Log.Information("Camera initialized successfully.");
                }

                // Validate EffectsManager
                if(EffectsManager == null) {
                    Log.Error("EffectsManager initialization failed. EffectsManager is null.");
                }
                else {
                    Log.Information("EffectsManager is successfully initialized.");
                }
            } catch(Exception ex) {
                Log.Error(ex, "An error occurred during _3DViwerViewModel initialization.");
            }
        }

        // Generate Geometry from G-code
        private LineGeometry3D GenerateGcodeGeometry(string fileContent) {
            var builder = new LineBuilder();
            var currentPosition = new Vector3(0, 0, 0);

            foreach(var line in fileContent.Split(Environment.NewLine)) {
                try {
                    if(string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith(";")) // Skip comments and empty lines
{
                        continue;
                    }

                    if(line.StartsWith("G1")) // Parse movement commands
                    {
                        var parts = line.Split(' ');
                        float x = currentPosition.X;
                        float y = currentPosition.Y;
                        float z = currentPosition.Z;

                        foreach(var part in parts) {
                            if(part.StartsWith("X") && float.TryParse(part.Substring(1), out float parsedX)) {
                                x = parsedX;
                            }

                            if(part.StartsWith("Y") && float.TryParse(part.Substring(1), out float parsedY)) {
                                y = parsedY;
                            }

                            if(part.StartsWith("Z") && float.TryParse(part.Substring(1), out float parsedZ)) {
                                z = parsedZ;
                            }
                        }

                        var newPosition = new Vector3(x, y, z);
                        builder.AddLine(currentPosition, newPosition);
                        currentPosition = newPosition;

                        Log.Debug("Parsed line successfully: {LineContent}, New Position: {NewPosition}", line, newPosition);
                    }
                } catch(Exception ex) {
                    Log.Warning(ex, "Error parsing line: {LineContent}", line);
                }
            }

            return builder.ToLineGeometry3D();
        }
    }
}