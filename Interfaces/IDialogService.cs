namespace NanoFlow.Interfaces {

    public interface IDialogService {

        Task<StlSettingsDialogViewModel?> ShowStlDialog(XamlRoot xamlRoot);

        Task<GcodeDialogViewModel?> ShowGCodeDialogAsync(XamlRoot xamlRoot);

        Task<bool> ShowGuidedProcessDialogAsync(List<PointModel> points, Canvas canvas, XamlRoot xamlRoot);

    }
}
