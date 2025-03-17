
namespace NanoFlow.Helpers;

public class NotificationHelper {

    public void LaunchToastNotification(string filename, string filepath) {

        ToastNotificationManagerCompat.OnActivated +=
            toastArgs => {

                ToastArguments arguments = ToastArguments.Parse(
                    toastArgs.Argument);

                if(arguments.TryGetValue("action", out string action)) {
                    HandleToastButtonAction(action, filename);
                }

                // Handle the toast button action
                HandleToastButtonAction(action, filename);
            };

        Toast(filename, filepath);
    }

    private void HandleToastButtonAction(string action, string filename) {
        switch(action) {

            case "openFile":
                break;

            case
                "openFolder":
                OpenFolder(Path.GetFullPath(filename));
                break;
            default:
                break;
        }
    }

    private void OpenFolder(string path) {
        if(!Directory.Exists(path)) {
            return;
        }
        Process.Start("explorer.exe", $"/select,\"{path}\"");
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
