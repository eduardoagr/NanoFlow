using Microsoft.UI.Xaml.Media.Animation;

using Point = Windows.Foundation.Point;

namespace NanoFlow.Views;

public sealed partial class FileExplorerWindow : Window {

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

            string folderName = folderItem.FileName;
            int folderCount = FolderService.GetgcodeItems(folderName).Count;

            // Calculate the position of the hovered item
            var transform = container.TransformToVisual(rootContainer);
            var pos = transform.TransformPoint(new Point
                (0, container.ActualHeight - 36));

            FloatingPanel.UpdateContent(folderCount);

            FloatingPanel.RenderTransform = new TranslateTransform {
                X = pos.X,
                Y = pos.Y
            };

            FloatingPanel.ShowPanel();
        }
    }

    private void ItemContainer_PointerExited(object sender, PointerRoutedEventArgs e) {

        FloatingPanel.HidePanel();
    }
}
