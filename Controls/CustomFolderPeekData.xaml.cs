namespace NanoFlow.Controls {

    public sealed partial class CustomFolderPeekData : UserControl {

        public CustomFolderPeekData() {

            InitializeComponent();
        }

        public void UpdateContent(string CreationDate, string Name, string Count, string Size,
            string Owner) {

            FolderCreationDate.Text = $"Creation dete : {CreationDate}";
            FolderName.Text = $"Name : {Name}";
            FolderCount.Text = $"Files : {Count}";
            FoldeSize.Text = $"Size : {Size}";
            FolderOwner.Text = $"Owner : {Owner.Split('\\')[0]}";
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
