namespace NanoFlow.Model {
    public partial class GCodeModel : ObservableObject {

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        public string fileName = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        public string layerHeight = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        public string extruderTemp = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        public string bedTemp = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        public string retraction = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        public string extrusionLength = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        public double extrusionLengthExtended = 0;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        public string printSpeed = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        public string coolingSpeed = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        public string selectedPrinterModel;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        public string selectedFilament;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        public string selectedNozzleSize;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        public string selectedBedLeveling;

        public ObservableCollection<string> PrinterModels { get; } = ["Prusa i3", "Ender 3", "Anycubic Vyper", "Other"];
        public ObservableCollection<string> FilamentTypes { get; } = ["PLA", "ABS", "PETG", "TPU"];
        public ObservableCollection<string> NozzleSizes { get; } = ["0.2", "0.4", "0.6", "0.8"];
        public ObservableCollection<string> BedLevelingOptions { get; } = ["No", "Yes (G29)"];

        partial void OnExtrusionLengthChanged(string value) {

            extrusionLengthExtended = double.Parse(value);
            OnPropertyChanged(nameof(CanSave));
        }

        public bool CanSave =>
            !string.IsNullOrEmpty(FileName) &&
            !string.IsNullOrEmpty(LayerHeight) &&
            !string.IsNullOrEmpty(ExtruderTemp) &&
            !string.IsNullOrEmpty(BedTemp) &&
            !string.IsNullOrEmpty(Retraction) &&
            !string.IsNullOrEmpty(ExtrusionLength) &&
            ExtrusionLengthExtended > 0 &&
            !string.IsNullOrEmpty(PrintSpeed) &&
            !string.IsNullOrEmpty(CoolingSpeed) &&
            !string.IsNullOrEmpty(SelectedPrinterModel) &&
            !string.IsNullOrEmpty(SelectedFilament) &&
            !string.IsNullOrEmpty(SelectedNozzleSize) &&
            !string.IsNullOrEmpty(SelectedBedLeveling);
    }
}
