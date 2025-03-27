namespace NanoFlow.ViewModels {

    public partial class _3DViwerViewModel : ObservableObject {

        [ObservableProperty]
        private string fileContent;

        [ObservableProperty]
        private int fieldOView = 120;

        [ObservableProperty]
        Brush lineColor = new SolidColorBrush(Colors.Blue);

        [ObservableProperty]
        Color selecterLineColor;

        // Camera for 3D visualization
        [ObservableProperty]
        public PerspectiveCamera camera;

        // Rendering effects manager
        public DefaultEffectsManager EffectsManager { get; } = new DefaultEffectsManager();

        // Main 3D content (e.g., parsed G-code lines)
        public LineGeometry3D Root { get; }

        public Vector3 ModelCentroid { get; } = new Vector3(0, 0, 0);

        public _3DViwerViewModel(GcodeItem gcodeContent) {
            FileContent = File.Exists(gcodeContent.FilePath)
                ? File.ReadAllText(gcodeContent.FilePath)
                : string.Empty;

            Root = GenerateGcodeGeometry(FileContent);

            Camera = new PerspectiveCamera {
                Position = new Vector3(0, 0, 100),
                LookDirection = new Vector3(0, 0, -100),
                UpDirection = new Vector3(0, 1, 0),
                FieldOfView = fieldOView,
            };

            OnLineColorChanged(LineColor);

        }

        private LineGeometry3D GenerateGcodeGeometry(string FileContent) {
            var builder = new LineBuilder();
            var currentPosition = new Vector3(0, 0, 0);

            foreach(var line in FileContent.Split(Environment.NewLine)) {
                try {
                    if(line.TrimStart().StartsWith(";")) // Skip comments
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
                            if(part.StartsWith("X")) {
                                x = float.Parse(part.Substring(1));
                            }

                            if(part.StartsWith("Y")) {
                                y = float.Parse(part.Substring(1));
                            }

                            if(part.StartsWith("Z")) {
                                z = float.Parse(part.Substring(1));
                            }
                        }

                        var newPosition = new Vector3(x, y, z);
                        builder.AddLine(currentPosition, newPosition);
                        currentPosition = newPosition;
                    }
                } catch(Exception ex) {
                    Debug.WriteLine($"Error parsing line: {line}. Exception: {ex.Message}");
                }
            }

            return builder.ToLineGeometry3D();
        }

        partial void OnLineColorChanged(Brush value) {

            if(value is SolidColorBrush solidColorBrush) {
                SelecterLineColor = solidColorBrush.Color;
            }
        }
    }
}