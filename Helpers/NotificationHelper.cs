namespace NanoFlow.Helpers;

public class NotificationHelper {

    //public void LaunchToastNotification(string filepath) {

    //    ToastNotificationManagerCompat.OnActivated +=
    //        toastArgs => {

    //            ToastArguments arguments = ToastArguments.Parse(
    //                toastArgs.Argument);
    //        };

    //    Toast(Path.GetFileName(filepath), filepath);
    //}

    ///private void HandleToastButtonAction(string action, string filepath) {
    //    switch(action) {

    //        case "openFile":
    //            break;

    //        case
    //            "openFolder":
    //            Debug.WriteLine("Open Folder button clicked.");
    //            OpenFolder(filepath);
    //            break;
    //        default:
    //            break;
    //    }
    //}

    // Display the toast notification
    public static void Toast(string fileName) {
        new ToastContentBuilder()
            .AddText("Success")
            .AddText($"'{fileName}' saved successfully.")
            .Show();
    }
}
