

namespace NanoFlow.Model {

    public partial class LineModel(PointModel start, PointModel end) : ObservableObject {

        [ObservableProperty]
        private PointModel start = start;

        [ObservableProperty]
        private PointModel end = end;
    }
}
