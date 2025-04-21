namespace NanoFlow.Helpers;

public static class WindowHelper {

    // Helper method to get the AppWindow from a given Window

    private static AppWindow? GetAppWindowFromWindow(WindowEx window) {
        if(window is not null) {

            var hwnd = WindowNative.GetWindowHandle(window);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            return AppWindow.GetFromWindowId(windowId);
        }
        return null;
    }

    // Method to configure a window to use a custom title bar

    public static void ConfigureCustomTitleBar(WindowEx window, string title) {

        if(window is not null) {

            window.ExtendsContentIntoTitleBar = true;

            window.Title = title;
            window.SetIcon(Constants.appIconPath);

            var titleBarViewModel = new CustomTitleBarViewModel() {
                AppTitle = title,
            };

            var customTitleBar = new CustomTitleBar(titleBarViewModel) {
                Height = 48,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
            };

            var appWindow = GetAppWindowFromWindow(window);

            appWindow!.TitleBar.ButtonHoverForegroundColor = Colors.Transparent;
            appWindow!.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

            // Attach title bar dynamically
            var rootGrid = window.Content as Grid;
            if(rootGrid == null) {
                Debug.WriteLine("The window's content is not a Grid. Cannot add a custom title bar.");
                return;
            }

            rootGrid.Children.Insert(0, customTitleBar);

            // Set the custom title bar as the window's title bar
            window.SetTitleBar(customTitleBar);

        }
    }


    // Method to create a new window, add custom title bar, and optionally maximize
    public static Window CreateNewWindow(WindowEx newWindow, string title,
        bool maximize = false, double width = 1200, double height = 600, bool canMaximize = false, bool canResize = false) {

        if(maximize) {
            newWindow.Maximize();
        }
        newWindow.Height = height;
        newWindow.Width = width;
        newWindow.IsResizable = canMaximize;
        newWindow.IsMaximizable = canMaximize;
        newWindow.CenterOnScreen();

        newWindow.SystemBackdrop = new MicaBackdrop();

        // Configure the custom title bar for the new window
        ConfigureCustomTitleBar(newWindow, title);


        newWindow.Activate();

        return newWindow;
    }
}