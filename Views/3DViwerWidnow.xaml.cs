using Serilog;

namespace NanoFlow.Views {

    public sealed partial class _3DViwerWidnow : Window {

        public _3DViwerViewModel _3DViwerViewModel { get; set; }

        private LineGeometryModel3D lineModel;

        public _3DViwerWidnow(_3DViwerViewModel ViwerViewModel) {

            try {
                Log.Information("Initializing _3DViwerWidnow...");

                InitializeComponent();

                // Bind the ViewModel to the Window
                _3DViwerViewModel = ViwerViewModel;
                rootContainer.DataContext = _3DViwerViewModel;

                // Initialize the 3D content
                Initialize3DModel();

                Log.Information("Successfully initialized _3DViwerWidnow.");
            } catch(Exception ex) {
                Log.Error(ex, "Error initializing _3DViwerWidnow.");
            }

        }

        private void Initialize3DModel() {

            try {
                Log.Information("Setting up 3D model for rendering...");

                // Create a LineGeometryModel3D instance
                lineModel = new LineGeometryModel3D {
                    Geometry = _3DViwerViewModel.Root, // Assign geometry from the ViewModel
                    Thickness = 2.0,
                    Color = Colors.Green
                };

                // Add the model to the Viewport3DX
                Viewport3DX.Items.Add(lineModel);

                Log.Information("3D model successfully added to the viewport.");
            } catch(Exception ex) {
                Log.Error(ex, "Failed to set up 3D model for rendering.");
            }
        }
    }
}