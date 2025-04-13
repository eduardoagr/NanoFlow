namespace NanoFlow.Views {

    public sealed partial class _3DViwerWidnow : WindowEx {

        public _3DViwerViewModel _3DViwerViewModel { get; set; }

        private LineGeometryModel3D? lineModel;

        public _3DViwerWidnow(_3DViwerViewModel ViwerViewModel) {

            InitializeComponent();

            // Bind the ViewModel to the Window
            _3DViwerViewModel = ViwerViewModel;
            rootContainer.DataContext = _3DViwerViewModel;

            ViwerViewModel.errorPortMsg += async () => {
                // Show error message

                var messageDlg = new ContentDialog {
                    Title = "Error",
                    Content = "Please chech that you are conneted printed",
                    PrimaryButtonText = Constants.ok,
                    XamlRoot = Content.XamlRoot
                };

                await messageDlg.ShowAsync();

            };
            // Initialize the 3D content
            Initialize3DModel();

        }

        private void Initialize3DModel() {

            // Create a LineGeometryModel3D instance
            lineModel = new LineGeometryModel3D {
                Geometry = _3DViwerViewModel.Root, // Assign geometry from the ViewModel
                Thickness = 2.0,
                Color = Colors.GreenYellow
            };

            // Add the model to the Viewport3DX
            Viewport3DX.Items.Add(lineModel);
        }


    }
}