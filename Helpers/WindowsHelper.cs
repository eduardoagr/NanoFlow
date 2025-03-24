namespace NanoFlow.Helpers;

public static class WindowHelper {

    // Helper method to get the AppWindow from a given Window

    private static AppWindow? GetAppWindowFromWindow(Window window) {
        if(window is not null) {

            var hwnd = WindowNative.GetWindowHandle(window);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            return AppWindow.GetFromWindowId(windowId);
        }
        return null;
    }

    // Method to configure a window to use a custom title bar

    public static void ConfigureCustomTitleBar(Window window, string title) {

        if(window is not null) {

            window.ExtendsContentIntoTitleBar = true;

            var titleBarViewModel = new CustomTitleBarViewModel(window) {
                AppTitle = title,
            };

            Debug.WriteLine($"AppTitle set to: {titleBarViewModel.AppTitle}");

            var customTitleBar = new CustomTitleBar(window, titleBarViewModel) {
                DataContext = titleBarViewModel,
                VerticalAlignment = VerticalAlignment.Top,
            };

            window.ExtendsContentIntoTitleBar = true;

            var appWindow = GetAppWindowFromWindow(window);

            appWindow!.TitleBar.ButtonHoverForegroundColor = Colors.Transparent;
            appWindow!.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

            // Attach title bar dynamically
            var rootGrid = (Grid)window.Content;

            // Add the custom title bar to the grid
            rootGrid!.Children.Insert(0, customTitleBar);
        }
    }


    // Method to maximize a window using the AppWindow's OverlappedPresenter

    public static void MaximizeWindow(Window window) {
        if(window is not null) {

            // Get the AppWindow
            var appWindow = GetAppWindowFromWindow(window);

            var presenter = appWindow?.Presenter as OverlappedPresenter;
            presenter?.Maximize();
        }
    }

    // Method to create a new window, add custom title bar, and optionally maximize
    public static Window CreateNewWindow(Window newWindow, string title, bool maximize = false) {

        // Configure the custom title bar for the new window
        ConfigureCustomTitleBar(newWindow, title);

        if(maximize) {
            MaximizeWindow(newWindow);
        }

        newWindow.Activate();

        return newWindow;
    }
}