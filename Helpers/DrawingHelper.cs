using Color = Windows.UI.Color;

namespace NanoFlow.Helpers;

public class DrawingHelper(
    Canvas? canvas, Grid? rootContainer, List<Line> gridMarginLines,
    List<TextBlock> gridTextBlocks, List<PointModel> selectedPoints,
    ObservableCollection<LineModel> lines) {

    public Canvas CreateCanva() {

        canvas = new Canvas {
            Background = new SolidColorBrush(Color.FromArgb(255, 0, 128, 0)),
            Width = Constants._canvasSize,
            Height = Constants._canvasSize,
        };

        rootContainer!.Children.Add(canvas);
        Grid.SetRow(canvas, 2);

        return canvas;
    }

    public void DrawPoint(PointModel point) {
        if(selectedPoints.Contains(point)) {

            var toRemove = canvas!.Children.OfType<Ellipse>().FirstOrDefault(e => e.Tag == point);
            if(toRemove != null) {
                canvas.Children.Remove(toRemove);
            }

            // Remove the point from the selected points list
            selectedPoints.Remove(point);
        }
        else {

            var ellipse = new Ellipse {
                Width = 4,
                Height = 4,
                Fill = new SolidColorBrush(Colors.Red)
            };
            Canvas.SetLeft(ellipse, point.X - 2);
            Canvas.SetTop(ellipse, point.Y - 2);
            canvas!.Children.Add(ellipse);

            // Connect with the previous point if one exists
            if(selectedPoints.Count > 0) {
                var lastPoint = selectedPoints[^1];
                DrawLine(lastPoint, point);
            }

            selectedPoints.Add(point);
        }
    }

    public void DrawLine(PointModel startPoint, PointModel endPoint) {
        Line line = new() {
            X1 = startPoint.X,
            Y1 = startPoint.Y,
            X2 = endPoint.X,
            Y2 = endPoint.Y,
            Stroke = new SolidColorBrush(Colors.White),
            StrokeThickness = 2
        };

        canvas!.Children.Add(line);
        lines.Add(new LineModel(startPoint, endPoint));
    }

    public void DrawGridMarginsAndTextBlocks() {
        if(canvas == null) {
            return;
        }

        for(int i = 0; i < canvas!.Width; i += Constants.gridSpacing) {
            for(int j = 0; j < canvas.Height; j += Constants.gridSpacing) {
                // Draw vertical and horizontal lines crossing at intersections
                if(i == 0) {
                    var horizontalLine = new Line {
                        X1 = 0,
                        Y1 = j,
                        X2 = canvas.Width,
                        Y2 = j,
                        Stroke = new SolidColorBrush(Colors.Gray),
                        StrokeThickness = 1
                    };
                    canvas.Children.Add(horizontalLine);
                    gridMarginLines.Add(horizontalLine);
                }

                if(j == 0) {
                    var verticalLine = new Line {
                        X1 = i,
                        Y1 = 0,
                        X2 = i,
                        Y2 = canvas.Height,
                        Stroke = new SolidColorBrush(Colors.Gray),
                        StrokeThickness = 1
                    };
                    canvas.Children.Add(verticalLine);
                    gridMarginLines.Add(verticalLine);
                }

                // Add a TextBlock at each intersection
                var textBlock = new TextBlock {
                    Text = $"{i / Constants.gridSpacing},{j / Constants.gridSpacing}",
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 6.5
                };
                Canvas.SetLeft(textBlock, i + 2); // Position slightly offset for visibility
                Canvas.SetTop(textBlock, j + 2);

                canvas.Children.Add(textBlock);
                gridTextBlocks.Add(textBlock);
            }
        }
    }

    public void RemoveGridMarginsAndTextBlocks() {
        if(canvas == null) {
            return;
        }

        var allItems = gridMarginLines.Cast<UIElement>()
            .Concat(gridTextBlocks.Cast<UIElement>())
            .ToList();

        foreach(var element in allItems) {
            canvas.Children.Remove(element);
        }

        gridMarginLines.Clear();
        gridTextBlocks.Clear();
    }
}
