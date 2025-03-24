namespace NanoFlow.Helpers;

public class NotificationHelper {

    public void LaunchToastNotification(string filepath) {

        ToastNotificationManagerCompat.OnActivated +=
            toastArgs => {

                ToastArguments arguments = ToastArguments.Parse(
                    toastArgs.Argument);

                if(arguments.TryGetValue("action", out string action)) {
                    HandleToastButtonAction(action, filepath);
                }
            };

        Toast(Path.GetFileName(filepath), filepath);
    }

    private void HandleToastButtonAction(string action, string filepath) {
        switch(action) {

            case "openFile":
                break;

            case
                "openFolder":
                Debug.WriteLine("Open Folder button clicked.");
                OpenFolder(filepath);
                break;
            default:
                break;
        }
    }

    private void OpenFolder(string path) {
        Process.Start(Constants.explorer, $"/select,\"{path}\"");
    }

    // Display the toast notification
    private static void Toast(string fileName, string filepath) {
        new ToastContentBuilder()
            .AddText("Success")
            .AddText($"'{fileName}' saved successfully.")
            .AddButton(new ToastButton()
                .SetContent("Open File")
                .AddArgument("action", "openFile")
                .AddArgument("fileName", fileName)).
            AddButton(new ToastButton()
                .SetContent("Open Folder")
                .AddArgument("action", "openFolder")
                .AddArgument("filepath", filepath))
            .Show();
    }
}
