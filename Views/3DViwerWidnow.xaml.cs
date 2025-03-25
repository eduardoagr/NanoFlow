namespace NanoFlow.Views {

    public sealed partial class _3DViwerWidnow : Window {

        public _3DViwerViewModel _3DViwerViewModel { get; set; }

        public _3DViwerWidnow(_3DViwerViewModel ViwerViewModel) {
            InitializeComponent();

            _3DViwerViewModel = ViwerViewModel;

            rootContainer.DataContext = _3DViwerViewModel;

        }
    }
}
