using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace NanoFlow.ViewModels;

public partial class MainViewModel : ObservableObject {

    #region variables and constants
    private const string NanoFlowFolder = "NanoFlow";

    private const int gridSpacing = 20; // Spacing between grid lines in pixels

    private readonly int _canvasSize = 378; // Size of the canvas in pixels

    private bool _areGridMarginsAdded = false;

    private readonly List<Line> _gridMarginLines = []; // To track grid lines

    private readonly List<TextBlock> _gridTextBlocks = [];

    private Grid? _rootContainer;
    private Canvas? _canvas;
    private PointModel? _previousPoint;

    public ObservableCollection<PointModel> Points = [];

    public ObservableCollection<LineModel> Lines = [];

    public List<Tuple<PointModel, PointModel>> LineData = [];

    public List<PointModel> _selectedPoints = [];
    #endregion
    public void SetRootContainer(object sender, RoutedEventArgs args) {
        _rootContainer = sender as Grid;
    }

    #region Relay commands
    [RelayCommand]
    async Task SaveToSTLAsync() {
        // Create the dialog
        var fileNameDialog = new ContentDialog {
            Title = "Save Design",
            Content = new StackPanel {
                Children =
                {
                    new TextBlock { Text = "Enter a name for your design:" },
                    new TextBox { PlaceholderText = "MyDesign", Name = "FileNameTextBox" }
                }
            },
            PrimaryButtonText = "Save",
            CloseButtonText = "Cancel",
            XamlRoot = _rootContainer?.XamlRoot
        };

        // Show the dialog
        var result = await fileNameDialog.ShowAsync();

        if(result == ContentDialogResult.Primary) {
            // Retrieve the TextBox value
            var stackPanel = (StackPanel)fileNameDialog.Content;
            var textBox = stackPanel.Children.OfType<TextBox>().First();
            var fileName = textBox.Text;

            // Validate the file name
            if(string.IsNullOrWhiteSpace(fileName)) {
                // Use a default file name if none is provided
                fileName = "MyDesign.stl";
            }
            else if(!fileName.EndsWith(".stl", StringComparison.OrdinalIgnoreCase)) {
                // Append the ".stl" extension if it's missing
                fileName += ".stl";
            }

            // Generate the STL file
            GetLineData(); // Ensure LineData is updated
            Toast(fileName);
            ExportSTL(fileName, 5.0);
        }
    }

    [RelayCommand]
    async Task SaveGcodeAsync() {

        var viewModel = new GcodeDialogViewModel(); // Create the ViewModel for the dialog Set the DataContext

        var gcodeDialog = new GcodeSettingsDialog(viewModel) {

            XamlRoot = _rootContainer!.XamlRoot
        };

        var result = await gcodeDialog.ShowAsync();

        if(result == ContentDialogResult.Primary) {

            double selectedNozzleSize = double.Parse(viewModel.GCodeSettings.SelectedNozzleSize);

            double nozzleArea = Math.PI * Math.Pow(selectedNozzleSize / 2, 2);

            // Retrieve the values from the ViewModel
            var fileName = viewModel.GCodeSettings.FileName;
            var printerModel = viewModel.GCodeSettings.SelectedPrinterModel;
            var filamentType = viewModel.GCodeSettings.SelectedFilament;
            var nozzleSize = viewModel.GCodeSettings.SelectedNozzleSize;
            var layerHeight = viewModel.GCodeSettings.LayerHeight;
            var extruderTemp = viewModel.GCodeSettings.ExtruderTemp;
            var bedTemp = viewModel.GCodeSettings.BedTemp;
            var retraction = viewModel.GCodeSettings.Retraction;
            var printSpeed = viewModel.GCodeSettings.PrintSpeed;
            var bedLeveling = viewModel.GCodeSettings.SelectedBedLeveling;
            var coolingSpeed = viewModel.GCodeSettings.CoolingSpeed;

            var extrusionLength = viewModel.GCodeSettings.ExtrusionLengthExtended; // Placeholder value, should be adjusted dynamically based on movement
            var extruderAmount = nozzleArea * extrusionLength;

            GetLineData(); // Ensure LineData is updated

            // Assuming you already have a method for saving G-code with these parameters:
            ExportToGcode(fileName, printerModel, filamentType, nozzleSize, layerHeight,
                extruderTemp, bedTemp, retraction, printSpeed, bedLeveling, coolingSpeed, extruderAmount);

            Toast(fileName);
        }
    }

    [RelayCommand]
    void CreateNewDesign() {

        DrawCanvas();

        if(_canvas != null) {
            _canvas!.PointerPressed += Canvas_PointerPressed;
            _canvas.PointerMoved += Canvas_PointerMoved;
            _canvas.PointerReleased += Canvas_PointerReleased;
        }

        Points.Clear();
        Lines.Clear();
        LineData.Clear();
    }

    [RelayCommand]
    async Task GuidedProcessAsync() {

        Points.Clear();
        DrawCanvas();
        DrawGridMarginsAndTextBlocks();

        // Add labels at the intersection points 
        ContentDialog dlg = new() {
            XamlRoot = _rootContainer!.XamlRoot,
            Title = "Select point to draw",
            PrimaryButtonText = "Ok",
            MinHeight = 400,
            MinWidth = 400,
            Height = 600,
            Width = 400,
            MaxHeight = 800,
            MaxWidth = 600

        };
        GridView gridView = new() {
            Width = 400,
            Height = 600
        };

        for(int i = 0; i < _canvas!.Width; i += gridSpacing) {
            for(int j = 0; j < _canvas.Height; j += gridSpacing) {
                var point = new PointModel(i, j);
                var checkBox = new CheckBox {
                    Content = $"Point ({point.X / gridSpacing},{point.Y / gridSpacing})",
                    IsChecked = false, // Default to unchecked                    
                    Tag = point,
                };
                checkBox.Checked += GuidCheckBox_CheckedChanged;
                checkBox.Unchecked += GuidCheckBox_CheckedChanged;
                gridView.Items.Add(checkBox);
            }
        }
        dlg.Content = gridView;
        var response = await dlg.ShowAsync();

        if(response == ContentDialogResult.Primary) {
            if(_selectedPoints.Count > 1) {
                for(int i = 0; i < _selectedPoints.Count - 1; i++) {
                    var startPoint = new PointModel(_selectedPoints[i].X * gridSpacing, _selectedPoints[i].Y * gridSpacing);
                    var endPoint = new PointModel(_selectedPoints[i + 1].X * gridSpacing, _selectedPoints[i + 1].Y * gridSpacing);
                    DrawLine(startPoint, endPoint);
                    RemoveGridMarginsAndTextBlocks();
                }
            }
        }
    }

    [RelayCommand]
    void AddMargin() {
        if(_canvas == null) {
            return;
        }

        if(_areGridMarginsAdded) {
            RemoveGridMarginsAndTextBlocks();
            _areGridMarginsAdded = false;
        }
        else {
            DrawGridMarginsAndTextBlocks();
            _areGridMarginsAdded = true;
        }

    }
    #endregion



    private void RemoveGridMarginsAndTextBlocks() {
        var allItems = _gridMarginLines.Cast<UIElement>()
             .Concat(_gridTextBlocks.Cast<UIElement>())
             .ToList();

        foreach(var element in allItems) {
            _canvas!.Children.Remove(element);
        }

        // Clear both lists afterward
        _gridMarginLines.Clear();
        _gridTextBlocks.Clear();
    }

    public List<Tuple<PointModel, PointModel>> GetLineData() {

        LineData.Clear();
        foreach(var line in Lines) {
            LineData.Add(new Tuple<PointModel, PointModel>(line.Start, line.End));
        }

        return LineData;
    }

    private static PointModel3D Create3DPoint(PointModel point, double z)
        => new(point.X, point.Y, z);

    public List<Tuple<PointModel3D, PointModel3D, PointModel3D, PointModel3D>>
        Generate3DLines(double thickness) {
        List<Tuple<PointModel3D, PointModel3D, PointModel3D, PointModel3D>> extrudedLines = [];

        foreach(var line in LineData) {
            var startBase = Create3DPoint(line.Item1, 0);
            var endBase = Create3DPoint(line.Item2, 0);
            var startTop = Create3DPoint(line.Item1, thickness);
            var endTop = Create3DPoint(line.Item2, thickness);

            extrudedLines.Add(new Tuple<PointModel3D, PointModel3D, PointModel3D, PointModel3D>(
                startBase, endBase, endTop, startTop
            ));
        }

        return extrudedLines;
    }

    public void ExportSTL(string filePath, double thickness) {

        GetLineData();
        var extrudedLines = Generate3DLines(thickness);

        // Validate extrudedLines before proceeding
        if(extrudedLines.Count == 0) {
            throw new InvalidOperationException("No 3D geometry created. Check the logic in Generate3DLines.");
        }

        var desinsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), NanoFlowFolder);

        if(!Directory.Exists(desinsFolder)) {
            Directory.CreateDirectory(desinsFolder);
        }

        var stlFilePath = Path.Combine(desinsFolder, filePath);

        var stlContent = new StringWriter(CultureInfo.InvariantCulture);
        stlContent.WriteLine("solid Model");

        foreach(var rectangle in extrudedLines) {

            var p1 = rectangle.Item1;
            var p2 = rectangle.Item2;
            var p3 = rectangle.Item3;
            var p4 = rectangle.Item4;

            AddRectangleToSTL(stlContent, p1, p2, p3, p4);

        }

        stlContent.WriteLine("endsolid Model");

        File.WriteAllText(stlFilePath, stlContent.ToString());
    }

    public void ExportToGcode(string fileName, string printerModel,
        string filamentType, string nozzleSize, string layerHeight, string extruderTemp,
        string bedTemp, string retraction, string printSpeed,
        string bedLeveling, string coolingSpeed, double extruderAmount) {


        // Define the extrusion thickness (this could be dynamic or a constant)
        double thickness = double.Parse(layerHeight); // Use the layer height as thickness for extrusion

        // Get the 3D lines (extruded)
        var extrudedLines = Generate3DLines(thickness);

        // Prepare G-code file
        var gcodeContent = new StringWriter(CultureInfo.InvariantCulture);

        // Header
        gcodeContent.WriteLine("; Generated G-code");
        gcodeContent.WriteLine("; Printer Model: " + printerModel);
        gcodeContent.WriteLine("; Filament Type: " + filamentType);
        gcodeContent.WriteLine("; Nozzle Size: " + nozzleSize);
        gcodeContent.WriteLine("; Layer Height: " + layerHeight);
        gcodeContent.WriteLine("; Extruder Temperature: " + extruderTemp);
        gcodeContent.WriteLine("; Bed Temperature: " + bedTemp);
        gcodeContent.WriteLine("; Retraction: " + retraction);
        gcodeContent.WriteLine("; Print Speed: " + printSpeed);
        gcodeContent.WriteLine("; Bed Leveling: " + bedLeveling);
        gcodeContent.WriteLine("; Cooling Speed: " + coolingSpeed);

        // Set units to millimeters
        gcodeContent.WriteLine("G21 ; Set units to millimeters");
        gcodeContent.WriteLine("G90 ; Use absolute positioning");

        // Initial settings for extruder and bed
        gcodeContent.WriteLine($"M104 S{extruderTemp} ; Set extruder temperature to {extruderTemp}°C");
        gcodeContent.WriteLine($"M140 S{bedTemp} ; Set bed temperature to {bedTemp}°C");
        gcodeContent.WriteLine("M190 S" + bedTemp + " ; Wait for bed to reach temperature");

        // Define the nozzle and other parameters
        gcodeContent.WriteLine($"M107 ; Turn off fan");

        // Begin printing process (just a simple structure for demonstration)
        gcodeContent.WriteLine("G1 Z" + layerHeight + " ; Set initial layer height");

        // Start the printing loop (using your extruded 3D lines)
        foreach(var line in extrudedLines) {
            var startPoint = line.Item1;
            var endPoint = line.Item2;

            // Move to the start of the 3D line and extrude
            gcodeContent.WriteLine($"G1 X{startPoint.X} Y{startPoint.Y} Z{startPoint.Z} E{extruderAmount} ; Move to start point and extrude");
            gcodeContent.WriteLine($"G1 X{endPoint.X} Y{endPoint.Y} Z{endPoint.Z} E{extruderAmount} ; Draw line to end point and extrude");
        }

        // Add additional movement (like retraction, speed settings, etc.)
        gcodeContent.WriteLine($"G1 F{printSpeed} ; Set print speed");

        // End G-code (turn off heaters, etc.)
        gcodeContent.WriteLine("M104 S0 ; Turn off extruder");
        gcodeContent.WriteLine("M140 S0 ; Turn off bed");
        gcodeContent.WriteLine("M106 S0 ; Turn off fan");
        gcodeContent.WriteLine("G28 X0 Y0 ; Move to home position");
        gcodeContent.WriteLine("M84 ; Disable motors");

        // Save the generated G-code to a file
        var designsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), NanoFlowFolder);
        if(!Directory.Exists(designsFolder)) {
            Directory.CreateDirectory(designsFolder);
        }

        var gcodeFilePath = Path.Combine(designsFolder, fileName);
        File.WriteAllText(gcodeFilePath, gcodeContent.ToString());

        Toast(fileName);
    }


    private static void AddRectangleToSTL(StringWriter stl, PointModel3D p1, PointModel3D p2,
        PointModel3D p3, PointModel3D p4) {

        // Define two triangles for the rectangle
        AddTriangleToStl(stl, p1, p2, p3); // Triangle 1
        AddTriangleToStl(stl, p1, p3, p4); // Triangle 2
    }

    private static void AddTriangleToStl(StringWriter stl, PointModel3D v1,
        PointModel3D v2, PointModel3D v3) {
        stl.WriteLine("  facet normal 0 0 0"); // Normal vector (set to 0 for simplicity)
        stl.WriteLine("    outer loop");
        stl.WriteLine($"      vertex {v1.X} {v1.Y} {v1.Z}");
        stl.WriteLine($"      vertex {v2.X} {v2.Y} {v2.Z}");
        stl.WriteLine($"      vertex {v3.X} {v3.Y} {v3.Z}");
        stl.WriteLine("    endloop");
        stl.WriteLine("  endfacet");
    }

    private void Toast(string fileName) {

        // Define the toast content as a string
        string toastXmlString =
            "<toast>" +
            "<visual>" +
            "<binding template='ToastGeneric'>" +
            $"<text>Success</text>" +
            $"<text>'{fileName}' saved successfully.</text>" +
            "</binding>" +
            "</visual>" +
            "</toast>";

        // Create an XML document for the toast content
        XmlDocument toastXml = new();
        toastXml.LoadXml(toastXmlString);

        // Create the toast notification
        ToastNotification toast = new ToastNotification(toastXml);

        // Show the toast notification
        ToastNotificationManager.CreateToastNotifier().Show(toast);
    }

    #region Drawing Methods
    private void DrawCanvas() {

        _canvas = new Canvas {
            Background = new SolidColorBrush(Color.FromArgb(255, 0, 128, 0)),
            Width = _canvasSize,
            Height = _canvasSize,
        };

        if(_rootContainer != null) {
            _rootContainer.Children.Add(_canvas);
            Grid.SetRow(_canvas, 1);
        }
    }

    private void DrawGridMarginsAndTextBlocks() {
        if(_canvas == null) {
            return;
        }

        for(int i = 0; i < _canvas!.Width; i += gridSpacing) {
            for(int j = 0; j < _canvas.Height; j += gridSpacing) {
                // Draw vertical and horizontal lines crossing at intersections
                if(i == 0) {
                    var horizontalLine = new Line {
                        X1 = 0,
                        Y1 = j,
                        X2 = _canvas.Width,
                        Y2 = j,
                        Stroke = new SolidColorBrush(Colors.Gray),
                        StrokeThickness = 1
                    };
                    _canvas.Children.Add(horizontalLine);
                    _gridMarginLines.Add(horizontalLine);
                }

                if(j == 0) {
                    var verticalLine = new Line {
                        X1 = i,
                        Y1 = 0,
                        X2 = i,
                        Y2 = _canvas.Height,
                        Stroke = new SolidColorBrush(Colors.Gray),
                        StrokeThickness = 1
                    };
                    _canvas.Children.Add(verticalLine);
                    _gridMarginLines.Add(verticalLine);
                }

                // Add a TextBlock at each intersection
                var textBlock = new TextBlock {
                    Text = $"({i / gridSpacing},{j / gridSpacing})",
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 8
                };
                Canvas.SetLeft(textBlock, i + 2); // Position slightly offset for visibility
                Canvas.SetTop(textBlock, j + 2);

                _canvas.Children.Add(textBlock);
                _gridTextBlocks.Add(textBlock);
            }
        }
    }

    private void DrawLine(PointModel startPoint, PointModel endPoint) {
        var line = new Line {
            X1 = startPoint.X,
            Y1 = startPoint.Y,
            X2 = endPoint.X,
            Y2 = endPoint.Y,
            Stroke = new SolidColorBrush(Colors.White),
            StrokeThickness = 2,
        };
        _canvas!.Children.Add(line);

        Lines.Add(new LineModel(startPoint, endPoint));
    }

    private void DrawPoint(PointModel point) {

        if(_selectedPoints.Contains(point)) {

            var toRemove = _canvas!.Children.OfType<Ellipse>().FirstOrDefault(e => e.Tag == point);
            if(toRemove != null) {
                _canvas.Children.Remove(toRemove);
            }

            // Remove the point from the selected points list
            _selectedPoints.Remove(point);
        }
        else {

            var ellipse = new Ellipse {
                Width = 4,
                Height = 4,
                Fill = new SolidColorBrush(Colors.Red)
            };
            Canvas.SetLeft(ellipse, point.X - 2);
            Canvas.SetTop(ellipse, point.Y - 2);
            _canvas!.Children.Add(ellipse);

            // Connect with the previous point if one exists
            if(_selectedPoints.Count > 0) {
                var lastPoint = _selectedPoints[^1];
                DrawLine(lastPoint, point);
            }

            _selectedPoints.Add(point);
        }
    }

    #endregion

    #region UI Event Handlers

    private void GuidCheckBox_CheckedChanged(object sender, RoutedEventArgs e) {
        var checkBox = sender as CheckBox;
        if(checkBox?.Tag is PointModel point) {
            var correctedPoint = new PointModel(point.X / gridSpacing, point.Y / gridSpacing);
            if(checkBox!.IsChecked == true) {
                _selectedPoints.Add(correctedPoint);
            }
            else {
                _selectedPoints.Remove(correctedPoint);
            }
        }
    }

    private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e) {
        var position = e.GetCurrentPoint(_canvas).Position;

        // Create the PointModel with the action(s) required
        var currentPoint = new PointModel((int)position.X, (int)position.Y);

        // Set the previous point for line drawing
        _previousPoint = currentPoint;

        // Trigger the action to draw the point
        DrawPoint(currentPoint);
    }

    private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e) {
        if(_previousPoint != null && e.Pointer.IsInContact) {
            var position = e.GetCurrentPoint(_canvas).Position;

            // Create the PointModel dynamically with the actions
            var currentPoint = new PointModel((int)position.X, (int)position.Y);

            // Draw a line between the points
            DrawLine(_previousPoint, currentPoint);

            // Update the previous point
            _previousPoint = currentPoint;
        }
    }

    private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e) {
        // Reset the previous point to stop drawing
        _previousPoint = null;
    }

    #endregion
}
