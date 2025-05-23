﻿using System.Management;

namespace NanoFlow.ViewModels;

public partial class MainViewModel(IServiceProvider serviceProvider,
    IDialogService _dialogService) : ObservableObject {

    #region variables and constants

    [ObservableProperty]
    bool isComPortConnected = false;

    private bool _areGridMarginsAdded = false;
    private Grid? _rootContainer;
    private Canvas? _canvas;
    private PointModel? _previousPoint;
    private DrawingHelper? _drawingHelper;

    private readonly List<Line> _gridMarginLines = [];

    private readonly List<TextBlock> _gridTextBlocks = [];

    public ObservableCollection<PointModel> Points = [];

    public ObservableCollection<LineModel> Lines = [];

    public List<Tuple<PointModel, PointModel>> LineData = [];

    public ObservableCollection<string> ComPorts = [];

    public event Action<ObservableCollection<string>> ComPortsListAction;

    public List<PointModel> _selectedPoints = [];

    [ObservableProperty]
    string comPortStatus = Constants.discconnectedPrinter;

    #endregion

    public void SetRootContainer(object sender, RoutedEventArgs args) {
        _rootContainer = sender as Grid;

        Init();
    }

    private void Init() {

        _drawingHelper = new(_canvas,
            _rootContainer, _gridMarginLines,
            _gridTextBlocks, _selectedPoints, Lines);
    }

    #region Relay commands

    [RelayCommand]
    void GetComPortsCommand() {
        GetComPorts();
        ComPortsListAction?.Invoke(ComPorts);
    }

    [RelayCommand]
    async Task SaveGcodeAsync() {

        if(_rootContainer?.XamlRoot == null) {
            throw new InvalidOperationException("XamlRoot is not set.");
        }

        var gcodeViewModel = await _dialogService.ShowGCodeDialogAsync(_rootContainer.XamlRoot);

        if(gcodeViewModel is not null) {

            double selectedNozzleSize = double.Parse(gcodeViewModel.GCodeSettings.SelectedNozzleSize);
            double nozzleArea = Math.PI * Math.Pow(selectedNozzleSize / 2, 2);

            var fileName = gcodeViewModel.GCodeSettings.FileName;

            if(!fileName.EndsWith(Constants.gcode, StringComparison.OrdinalIgnoreCase)) {

                fileName += Constants.gcode;
            }

            GetLineData();

            ExportToGcode(
                fileName,
                gcodeViewModel.GCodeSettings.SelectedPrinterModel,
                gcodeViewModel.GCodeSettings.SelectedFilament,
                gcodeViewModel.GCodeSettings.SelectedNozzleSize,
                gcodeViewModel.GCodeSettings.LayerHeight,
                gcodeViewModel.GCodeSettings.ExtruderTemp,
                gcodeViewModel.GCodeSettings.BedTemp,
                gcodeViewModel.GCodeSettings.Retraction,
                gcodeViewModel.GCodeSettings.PrintSpeed,
                gcodeViewModel.GCodeSettings.SelectedBedLeveling,
                gcodeViewModel.GCodeSettings.CoolingSpeed,
                nozzleArea * gcodeViewModel.GCodeSettings.ExtrusionLengthExtended);

            NotificationHelper.Toast(fileName);
        }
    }

    [RelayCommand]
    void CreateNewDesign() {

        _canvas = _drawingHelper!.CreateCanva();

        if(_canvas != null) {
            _canvas!.PointerPressed += Canvas_PointerPressed;
            _canvas.PointerMoved += Canvas_PointerMoved;
            _canvas.PointerReleased += Canvas_PointerReleased;
        }

        Points.Clear();
        Lines.Clear();
        LineData.Clear();
        _selectedPoints.Clear();
    }

    [RelayCommand]
    async Task GuidedProcessAsync() {

        Points.Clear();
        _selectedPoints.Clear();
        _canvas = _drawingHelper!.CreateCanva();
        _drawingHelper.DrawGridMarginsAndTextBlocks();

        if(_canvas == null || _rootContainer?.XamlRoot == null) {
            throw new InvalidOperationException("Canvas or XamlRoot is not initialized.");
        }

        var isConfirmed = await _dialogService.ShowGuidedProcessDialogAsync(_selectedPoints, _canvas, _rootContainer.XamlRoot);

        if(isConfirmed && _selectedPoints.Count > 1) {

            for(int i = 0; i < _selectedPoints.Count - 1; i++) {
                var startPoint = new PointModel(
                    _selectedPoints[i].X * Constants.gridSpacing,
                    _selectedPoints[i].Y * Constants.gridSpacing
                );

                var endPoint = new PointModel(
                    _selectedPoints[i + 1].X * Constants.gridSpacing,
                    _selectedPoints[i + 1].Y * Constants.gridSpacing
                );

                _drawingHelper.DrawLine(startPoint, endPoint);
            }
        }
        _drawingHelper.RemoveGridMarginsAndTextBlocks();
    }

    [RelayCommand]
    void OpenFileExplorer() {

        var searchFilesWindow = serviceProvider.GetRequiredService<FileExplorerWindow>();

        WindowHelper.CreateNewWindow(searchFilesWindow, "Search for file");
    }


    [RelayCommand]
    void AddMargin() {
        if(_canvas == null) {
            return;
        }

        if(_areGridMarginsAdded) {
            _drawingHelper!.RemoveGridMarginsAndTextBlocks();
            _areGridMarginsAdded = false;
        }
        else {
            _drawingHelper!.DrawGridMarginsAndTextBlocks();
            _areGridMarginsAdded = true;
        }

    }
    #endregion

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
        var gcodeFilePath = FilePathManager.GetFilePath(fileName);
        File.WriteAllText(gcodeFilePath, gcodeContent.ToString());

    }

    public void GetComPorts() {
        ComPorts.Clear();
        try {

            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%'");
            foreach(var queryObj in searcher.Get()) {
                var deviceName = queryObj["Name"]?.ToString();
                if(deviceName != null) {
                    ComPorts.Add(deviceName);
                }
            }

        } catch(Exception) {

            throw;
        }

    }
    

    #region UI Event Handlers

    private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e) {
        var position = e.GetCurrentPoint(_canvas).Position;

        // Create the PointModel with the action(s) required
        var currentPoint = new PointModel((int)position.X, (int)position.Y);

        // Set the previous point for line drawing
        _previousPoint = currentPoint;

        // Trigger the action to draw the point
        _drawingHelper!.DrawPoint(currentPoint);
    }

    private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e) {
        if(_previousPoint != null && e.Pointer.IsInContact) {
            var position = e.GetCurrentPoint(_canvas).Position;

            // Create the PointModel dynamically with the actions
            var currentPoint = new PointModel((int)position.X, (int)position.Y);

            // Draw a line between the points
            _drawingHelper!.DrawLine(_previousPoint, currentPoint);

            // Update the previous point
            _previousPoint = currentPoint;
        }
    }

    private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e) {
        // Reset the previous point to stop drawing
        _previousPoint = null;
    }

    #endregion End o UI
}