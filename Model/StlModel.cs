namespace NanoFlow.Model {

    public partial class StlModel : ObservableObject {

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        string? filename;

        public bool CanSave =>
            !string.IsNullOrEmpty(Filename);
    }
}
