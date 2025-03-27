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

    private void AnimateDetailsPane(double targetOffset) {
        Storyboard storyboard = new();
        DoubleAnimation slideAnimation = new() {
            From = ((TranslateTransform)SlidingGrid.RenderTransform).X, // Start from current offset
            To = targetOffset, // Move to the target offset
            Duration = TimeSpan.FromSeconds(0.5),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
        };
        storyboard.Children.Add(slideAnimation);
        Storyboard.SetTarget(slideAnimation, SlidingGrid.RenderTransform);
        Storyboard.SetTargetProperty(slideAnimation, "X"); // String representation o
        storyboard.Begin();
    }
}
