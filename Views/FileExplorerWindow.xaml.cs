

using Point = Windows.Foundation.Point;

namespace NanoFlow.Views;

public sealed partial class FileExplorerWindow : WindowEx {

    public FileExplorerWindowViewModel FileExplorerWindowViewModel { get; set; }

    public FileExplorerWindow(FileExplorerWindowViewModel explorerWindowViewModel) {

        InitializeComponent();

        FileExplorerWindowViewModel = explorerWindowViewModel;

        FileExplorerWindowViewModel.OnDetailsPanelSlide += AnimateDetailsPane;

        rootContainer.DataContext = FileExplorerWindowViewModel;
    }

    private void AnimateDetailsPane(string fileName) {

        Storyboard storyboard = new();
        DoubleAnimation slideAnimation = new() {
            Duration = TimeSpan.FromSeconds(0.3),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut } // Optional easing for natural movement
        };

        if(!string.IsNullOrEmpty(fileName)) {
            // Slide the pane into view
            slideAnimation.From = 300; // Start off-screen
            slideAnimation.To = 0;     // End on-screen
            SlidingGrid.Visibility = Visibility.Visible; // Ensure it’s visible
        }

        // Apply the animation to the TranslateTransform
        Storyboard.SetTarget(slideAnimation, SlideTransform);
        Storyboard.SetTargetProperty(slideAnimation, "X");
        storyboard.Children.Add(slideAnimation);
        storyboard.Begin();
    }

    private void ItemContainer_PointerEntered(object sender, PointerRoutedEventArgs e) {

        if(sender is FrameworkElement container
            && container.DataContext is FolderItem folderItem) {

            var directoryInfo = new DirectoryInfo(folderItem.FilePath);

            var folderName = folderItem.FileName;
            var folderCount = FolderService.GetgcodeItems(folderName).Count;
            var FolderSize = FolderService.GetgcodeItems(folderName).Sum(x => x.FileSize);
            var FolderReadableFileSize = folderItem.FormatFileSize(FolderSize);
            var FolderCreationDate = directoryInfo.CreationTime;
            var directorySecurity = directoryInfo.GetAccessControl();
            string folderOwner = directorySecurity.GetOwner(typeof(System.Security.Principal.NTAccount))!.ToString();

            // Calculate the position of the hovered item
            var transform = container.TransformToVisual(rootContainer);
            var pos = transform.TransformPoint(new Point
                (0, container.ActualHeight));

            FloatingPanel.UpdateContent(FolderCreationDate.ToString("d"),
                folderName, folderCount.ToString(),
                FolderReadableFileSize, folderOwner);

            FloatingPanel.RenderTransform = new TranslateTransform {
                X = pos.X,
                Y = pos.Y + 10
            };

            FloatingPanel.IsHitTestVisible = false;

            FloatingPanel.ShowPanel();
        }
    }

    private void ItemContainer_PointerExited(object sender, PointerRoutedEventArgs e) {

        FloatingPanel.HidePanel();
    }
}
