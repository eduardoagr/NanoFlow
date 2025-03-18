namespace NanoFlow.Helpers {
    public class DialogHelper {

        public static async Task<ContentDialogResult> ShowDialogAsync(
           string title = "Default Title", UIElement? content = null,
           string primaryButtonText = "OK", string? closeButtonText = null,
           XamlRoot? xamlRoot = null, double? minHeight = null,
           double? minWidth = null, double? height = null, double? width = null,
           double? maxHeight = null, double? maxWidth = null, object? viewModel = null) {

            // Create the dialog
            var dlg = new ContentDialog {
                Title = title,
                Content = content ?? new TextBlock { Text = "No content provided." }, // Default content
                PrimaryButtonText = primaryButtonText,
                CloseButtonText = closeButtonText,
                XamlRoot = xamlRoot
            };

            // Set optional properties only if provided
            if(minHeight.HasValue) {
                dlg.MinHeight = minHeight.Value;
            }

            if(minWidth.HasValue) {
                dlg.MinWidth = minWidth.Value;
            }

            if(height.HasValue) {
                dlg.Height = height.Value;
            }

            if(width.HasValue) {
                dlg.Width = width.Value;
            }

            if(maxHeight.HasValue) {
                dlg.MaxHeight = maxHeight.Value;
            }

            if(maxWidth.HasValue) {
                dlg.MaxWidth = maxWidth.Value;
            }

            if(viewModel != null) {
                dlg.DataContext = viewModel; // Bind the ViewModel if provided
            }

            // Show the dialog and return the result
            return await dlg.ShowAsync();
        }
    }
}
