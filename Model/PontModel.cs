namespace NanoFlow.Model {

    public partial class PointModel(int x, int y) : ObservableObject {

        public int X { get; set; } = x;
        public int Y { get; set; } = y;

    }
}
