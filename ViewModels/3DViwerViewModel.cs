namespace NanoFlow.ViewModels {
    public partial class _3DViwerViewModel : ObservableObject {

        [ObservableProperty]
        private string fileContent;

        [ObservableProperty]
        private int fieldOfView = 160;

        [ObservableProperty]
        private string estimatedPrintTime;

        [ObservableProperty]
        public PerspectiveCamera camera = new() {
            Position = new Vector3(0, 0, 100),
            LookDirection = new Vector3(0, 0, -100),
            UpDirection = new Vector3(0, 1, 0),
            FieldOfView = 160
        };

        public DefaultEffectsManager EffectsManager { get; } = new DefaultEffectsManager();

        public LineGeometry3D Root { get; private set; }

        public Vector3 ModelCentroid { get; } = new Vector3(0, 0, 0);

        public _3DViwerViewModel(GcodeItem gcodeContent) {
            // Initialize properties
            fileContent = File.ReadAllText(gcodeContent!.FilePath);

            // Generate the geometry
            Root = GenerateGeometry(fileContent);

            float mediumSpeed = 75 * 60;
            float acceleration = 500;
            double estimatedTimeMinutes = CalculatePrintTime(fileContent, mediumSpeed, acceleration);

            if(estimatedTimeMinutes < 60) {
                EstimatedPrintTime = $"{Math.Round(estimatedTimeMinutes)} minutes";
            }
            else {
                double hours = Math.Floor(estimatedTimeMinutes / 60);
                double minutes = estimatedTimeMinutes % 60;
                EstimatedPrintTime = $"{hours} hours and {Math.Round(minutes)} minutes";
            }

        }

        private static LineGeometry3D GenerateGeometry(string fileContent) {
            var builder = new LineBuilder();
            var positions = ParseMovementCommands(fileContent);

            if(positions.Any()) {
                var previousPosition = positions.First();
                foreach(var position in positions.Skip(1)) {
                    builder.AddLine(previousPosition, position);
                    previousPosition = position;
                }
            }

            return builder.ToLineGeometry3D();
        }

        public static double CalculatePrintTime(string fileContent, float printSpeed, float acceleration = 500) {
            var positions = ParseMovementCommands(fileContent);
            double totalDistance = 0.0;
            double estimatedTime = 0.0;

            if(positions.Any()) {
                var previousPosition = positions.First(); // Initialize to the first position
                foreach(var position in positions.Skip(1)) {
                    double distance = Vector3.Distance(previousPosition, position);
                    double moveTime = distance / printSpeed;

                    // Add acceleration/deceleration time if applicable
                    if(acceleration > 0) {
                        double accelTime = printSpeed / acceleration; // Time to reach full speed
                        moveTime += accelTime;
                    }

                    totalDistance += distance;
                    estimatedTime += moveTime;
                    previousPosition = position;
                }
            }

            return estimatedTime; // Total print time in minutes
        }

        private static IEnumerable<Vector3> ParseMovementCommands(string fileContent) {
            var positions = new List<Vector3>();
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
                        if(part.StartsWith("X") && float.TryParse(part.AsSpan(1), out float parsedX)) { x = parsedX; }
                        if(part.StartsWith("Y") && float.TryParse(part.AsSpan(1), out float parsedY)) { y = parsedY; }
                        if(part.StartsWith("Z") && float.TryParse(part.AsSpan(1), out float parsedZ)) { z = parsedZ; }
                    }

                    currentPosition = new Vector3(x, y, z);
                    positions.Add(currentPosition); // Add the new position to the list
                }
            }

            return positions;
        }
    }
}