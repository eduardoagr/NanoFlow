namespace NanoFlow.Controls {

    public sealed partial class CustomFolderPeekData : UserControl {

        public CustomFolderPeekData() {

            InitializeComponent();
        }

        public void UpdateContent(int FolderCount) {

            FileNum.Text = $"Number of files: {FolderCount}";
        }

        public void ShowPanel() {
            VisualStateManager.GoToState(this, "peekContainerVisible",
                true);

            Visibility = Visibility.Visible;
        }

        public void HidePanel() {

            VisualStateManager.GoToState(this, "peekContainerCollapsed",
                true);

            DispatcherTimer timer = new() {
                Interval = TimeSpan.FromMilliseconds(300)
            };

            timer.Tick += (s, e) => {
                Visibility = Visibility.Collapsed;
                timer.Stop();
            };

            timer.Start();


        }

    }
}
