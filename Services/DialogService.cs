namespace NanoFlow.Services {

    public class DialogService(IServiceProvider serviceProvider) : IDialogService {
        public async Task<GcodeDialogViewModel?> ShowGCodeDialogAsync(XamlRoot xamlRoot) {

            var gcodeDialog = serviceProvider.GetRequiredService<GcodeSettingsDialog>();
            gcodeDialog.XamlRoot = xamlRoot;

            var result = await gcodeDialog.ShowAsync();

            if(result == ContentDialogResult.Primary && gcodeDialog.DataContext is GcodeDialogViewModel gcodeViewModel) {
                return gcodeViewModel;
            }

            return null; // User canceled or closed the dialog
        }

        public async Task<bool> ShowGuidedProcessDialogAsync(List<PointModel> points, Canvas canvas, XamlRoot xamlRoot) {

            GridView gridView = new() {
                Width = 400,
                Height = 600
            };

            for(int i = 0; i < canvas.Width; i += Constants.gridSpacing) {
                for(int j = 0; j < canvas.Height; j += Constants.gridSpacing) {

                    // Normalize the coordinates by dividing by grid spacing
                    var point = new PointModel(i / Constants.gridSpacing, j / Constants.gridSpacing);

                    var checkBox = new CheckBox {
                        Content = $"X: {point.X}, Y: {point.Y}",
                        IsChecked = false,
                        Tag = point
                    };

                    // Handle Check/Uncheck events to track selected points
                    checkBox.Checked += (s, e) => {
                        if(s is CheckBox cb && cb.Tag is PointModel p) {
                            points.Add(p);
                        }
                    };

                    checkBox.Unchecked += (s, e) => {
                        if(s is CheckBox cb && cb.Tag is PointModel p) {
                            points.Remove(p);
                        }
                    };

                    gridView.Items.Add(checkBox);
                }
            }

            // Create the ContentDialog
            var dialog = new ContentDialog {
                Title = Constants.pointsSeletion,
                Content = gridView,
                PrimaryButtonText = Constants.ok,
                CloseButtonText = Constants.cancel,
                XamlRoot = xamlRoot,
                MinWidth = 800,
                MaxWidth = 800,
                MaxHeight = 800,
                Height = 800,
                CornerRadius = new CornerRadius(8),
                Width = 600,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(10) // Ensure it is flush to the top-left

            };

            // Show the dialog and return selected points if confirmed
            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }
    }
}
