namespace NanoFlow.Model {
    public partial class GCodeModel : ObservableObject {

        [ObservableProperty] public string fileName = string.Empty;
        [ObservableProperty] public string layerHeight = string.Empty;
        [ObservableProperty] public string extruderTemp = string.Empty;
        [ObservableProperty] public string bedTemp = string.Empty;
        [ObservableProperty] public string retraction = string.Empty;
        [ObservableProperty] public string extrusionLength = string.Empty;
        [ObservableProperty] public double extrusionLengthExtended = 0;
        [ObservableProperty] public string printSpeed = string.Empty;
        [ObservableProperty] public string coolingSpeed = string.Empty;

        [ObservableProperty] public string selectedPrinterModel;
        [ObservableProperty] public string selectedFilament;
        [ObservableProperty] public string selectedNozzleSize;
        [ObservableProperty] public string selectedBedLeveling;

        public ObservableCollection<string> PrinterModels { get; } = ["Prusa i3", "Ender 3", "Anycubic Vyper", "Other"];
        public ObservableCollection<string> FilamentTypes { get; } = ["PLA", "ABS", "PETG", "TPU"];
        public ObservableCollection<string> NozzleSizes { get; } = ["0.2", "0.4", "0.6", "0.8"];
        public ObservableCollection<string> BedLevelingOptions { get; } = ["No", "Yes (G29)"];

        partial void OnExtrusionLengthChanged(string value) {

            extrusionLengthExtended = double.Parse(value);

        }
    }
}
