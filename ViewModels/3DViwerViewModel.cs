namespace NanoFlow.ViewModels {
    public partial class _3DViwerViewModel(GcodeItem gcodeContent) : ObservableObject {

        [ObservableProperty]
        private string fileContent = File.ReadAllText(gcodeContent!.FilePath);

        [ObservableProperty]
        private int fieldOfView = 160;

        [ObservableProperty]
        public PerspectiveCamera camera = new() {
            Position = new Vector3(0, 0, 100),
            LookDirection = new Vector3(0, 0, -100),
            UpDirection = new Vector3(0, 1, 0),
            FieldOfView = 160
        };

        // Effects Manager for 3D rendering
        public DefaultEffectsManager EffectsManager { get; } = new DefaultEffectsManager();

        // Main geometry to render
        public LineGeometry3D Root { get; } = GenerateGeometry(File.ReadAllText(gcodeContent!.FilePath));


        // Central rotation point for 3D view
        public Vector3 ModelCentroid { get; } = new Vector3(0, 0, 0);

        // Static method to generate geometry from G-code
        private static LineGeometry3D GenerateGeometry(string fileContent) {
            var builder = new LineBuilder();
            var currentPosition = new Vector3(0, 0, 0);

            foreach(var line in fileContent.Split(Environment.NewLine)) {
                if(string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith(";")) // Skip comments or empty lines
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
                }
            }

            return builder.ToLineGeometry3D();
        }
    }
}