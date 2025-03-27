using Microsoft.UI.Xaml.Media.Animation;

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
            Duration = TimeSpan.FromSeconds(0.5), // Adjust duration for smoothness
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut } // Optional easing for natural movement
        };

        // Check fileName validity to control animation direction
        if(!string.IsNullOrEmpty(fileName)) {
            // Slide the pane into view
            slideAnimation.From = 300; // Start off-screen
            slideAnimation.To = 0;     // End on-screen
            SlidingGrid.Visibility = Visibility.Visible; // Ensure it’s visible
        }
        else {
            // Slide the pane out of view
            slideAnimation.From = 0;   // Start on-screen
            slideAnimation.To = 300; // End off-screen
            SlidingGrid.Visibility = Visibility.Visible; // Still visible during animation
        }

        // Apply the animation to the TranslateTransform
        Storyboard.SetTarget(slideAnimation, SlideTransform);
        Storyboard.SetTargetProperty(slideAnimation, "X");
        storyboard.Children.Add(slideAnimation);
        storyboard.Begin();
    }
}
