using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        private Point? dragStart;
        private Shape selectedShape;
        private Point? panStart;
        private double offsetX = 0;
        private double offsetY = 0;
        private double drawX = 200;
        private double drawY = 150;

        public MainWindow()
        {
            InitializeComponent();
            if (ShapeComboBox != null)
                ShapeComboBox.SelectedIndex = 0;
            if (CoordXBox != null)
                CoordXBox.Text = drawX.ToString(CultureInfo.InvariantCulture);
            if (CoordYBox != null)
                CoordYBox.Text = drawY.ToString(CultureInfo.InvariantCulture);
            if (canvas2D != null)
            {
                canvas2D.RenderTransform = new MatrixTransform(); 
                canvas2D.MouseDown += Canvas_MouseDown;
                canvas2D.MouseMove += Canvas_MouseMove;
                canvas2D.MouseUp += Canvas_MouseUp;
                canvas2D.SizeChanged += (s, e) => CenterAxes();
                canvas2D.Loaded += (s, e) => CenterAxes();
            }
        }
        private void CenterAxes()
        {
            offsetX = 0;
            offsetY = 0;
            UpdateCanvasTransform();
        }

        private void CoordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CoordXBox == null || CoordYBox == null)
                return;
            if (double.TryParse(CoordXBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double x))
                drawX = x;
            if (double.TryParse(CoordYBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double y))
                drawY = y;
        }

        private void DrawAxes()
        {
            if (canvas2D == null) return;
            for (int i = canvas2D.Children.Count - 1; i >= 0; i--)
            {
                if (canvas2D.Children[i] is Line line && (string)line.Tag == "Axis")
                    canvas2D.Children.RemoveAt(i);
            }
            double width = canvas2D.ActualWidth;
            double height = canvas2D.ActualHeight;
            if (width <= 0 || height <= 0) return;

            double axisLength = Math.Max(width, height) * 10;
            double centerX = width / 2;
            double centerY = height / 2;

            var xAxis = new Line
            {
                X1 = centerX - axisLength / 2,
                Y1 = centerY,
                X2 = centerX + axisLength / 2,
                Y2 = centerY,
                Stroke = Brushes.Gray,
                StrokeThickness = 3,
                Tag = "Axis"
            };
            var yAxis = new Line
            {
                X1 = centerX,
                Y1 = centerY - axisLength / 2,
                X2 = centerX,
                Y2 = centerY + axisLength / 2,
                Stroke = Brushes.Gray,
                StrokeThickness = 3,
                Tag = "Axis"
            };
            canvas2D.Children.Insert(0, xAxis);
            canvas2D.Children.Insert(0, yAxis);
        }

        private void Mode2DButton_Checked(object sender, RoutedEventArgs e)
        {
            if (Mode3DButton != null)
                Mode3DButton.IsChecked = false;
            if (canvas2D != null)
                canvas2D.Visibility = Visibility.Visible;
            if (canvas3DPlaceholder != null)
                canvas3DPlaceholder.Visibility = Visibility.Collapsed;
            UpdateShapeComboBox("2D");
        }

        private void Mode2DButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Mode3DButton != null && Mode3DButton.IsChecked != true)
                Mode2DButton.IsChecked = true;
        }

        private void Mode3DButton_Checked(object sender, RoutedEventArgs e)
        {
            if (Mode2DButton != null)
                Mode2DButton.IsChecked = false;
            if (canvas2D != null)
                canvas2D.Visibility = Visibility.Collapsed;
            if (canvas3DPlaceholder != null)
                canvas3DPlaceholder.Visibility = Visibility.Visible;
            UpdateShapeComboBox("3D");
        }

        private void Mode3DButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Mode2DButton != null && Mode2DButton.IsChecked != true)
                Mode3DButton.IsChecked = true;
        }

        private void ShapeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ShapeComboBox?.SelectedItem is ComboBoxItem item)
            {
                string shape = item.Content?.ToString() ?? string.Empty;
                InputFieldsPanel.Children.Clear();
                switch (shape)
                {
                    case "Circle":
                        AddInputField("Radius:");
                        break;
                    case "Ellipse":
                        AddInputField("Radius X:");
                        AddInputField("Radius Y:");
                        break;
                    case "Rectangle":
                        AddInputField("Width:");
                        AddInputField("Height:");
                        break;
                    case "Triangle":
                        AddInputField("Base:");
                        AddInputField("Height:");
                        break;
                    case "Sphere":
                        AddInputField("Radius:");
                        break;
                    case "Cube":
                        AddInputField("Side:");
                        break;
                    case "Pyramid":
                        AddInputField("Base Width:");
                        AddInputField("Base Length:");
                        AddInputField("Height:");
                        break;
                }
            }
        }

        private void AddInputField(string label)
        {
            var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 0) };
            panel.Children.Add(new TextBlock { Text = label, Width = 80, VerticalAlignment = VerticalAlignment.Center });
            panel.Children.Add(new TextBox { Width = 100 });
            InputFieldsPanel.Children.Add(panel);
        }

        private void UpdateShapeComboBox(string mode)
        {
            if (ShapeComboBox == null) return;
            foreach (ComboBoxItem item in ShapeComboBox.Items)
            {
                string tag = item.Tag?.ToString();
                if (mode == "2D" && tag == "3D")
                    item.Visibility = Visibility.Collapsed;
                else if (mode == "3D" && tag == "2D")
                    item.Visibility = Visibility.Collapsed;
                else
                    item.Visibility = Visibility.Visible;
            }
            foreach (ComboBoxItem item in ShapeComboBox.Items)
            {
                if (item.Visibility == Visibility.Visible)
                {
                    ShapeComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void LimitShapeSize(string shape, double[] values)
        {
            double width = canvas2D.ActualWidth;
            double height = canvas2D.ActualHeight;
            double centerX = width / 2;
            double centerY = height / 2;

            switch (shape)
            {
                case "Circle":
                    double maxRadius = Math.Min(centerX, centerY) - 2;
                    if (values[0] > maxRadius) values[0] = maxRadius;
                    if (values[0] < 1) values[0] = 1;
                    break;
                case "Ellipse":
                    double maxRadiusX = centerX - 2;
                    double maxRadiusY = centerY - 2;
                    if (values[0] > maxRadiusX) values[0] = maxRadiusX;
                    if (values[1] > maxRadiusY) values[1] = maxRadiusY;
                    if (values[0] < 1) values[0] = 1;
                    if (values[1] < 1) values[1] = 1;
                    break;
                case "Rectangle":
                    double maxW = width - 4;
                    double maxH = height - 4;
                    if (values[0] > maxW) values[0] = maxW;
                    if (values[1] > maxH) values[1] = maxH;
                    if (values[0] < 1) values[0] = 1;
                    if (values[1] < 1) values[1] = 1;
                    break;
                case "Triangle":
                    double maxBase = width - 4;
                    double maxHeight = height - 4;
                    if (values[0] > maxBase) values[0] = maxBase;
                    if (values[1] > maxHeight) values[1] = maxHeight;
                    if (values[0] < 1) values[0] = 1;
                    if (values[1] < 1) values[1] = 1;
                    break;
            }
        }

        private void AddShape_Click(object sender, RoutedEventArgs e)
        {
            if (canvas2D == null || canvas2D.Visibility != Visibility.Visible)
            {
                MessageBox.Show("3D mode is a placeholder.");
                return;
            }

            if (ShapeComboBox?.SelectedItem is not ComboBoxItem item)
                return;

            string shape = item.Content?.ToString() ?? string.Empty;
            double[] values = new double[InputFieldsPanel.Children.Count];
            for (int i = 0; i < InputFieldsPanel.Children.Count; i++)
            {
                if (InputFieldsPanel.Children[i] is StackPanel panel &&
                    panel.Children[1] is TextBox tb)
                {
                    if (!double.TryParse(tb.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double val) || val <= 0)
                    {
                        MessageBox.Show("Please enter valid positive numbers for all fields.");
                        return;
                    }
                    values[i] = val;
                }
            }

            LimitShapeSize(shape, values);

            Shape newShape = null;
            switch (shape)
            {
                case "Circle":
                    newShape = new Ellipse
                    {
                        Width = values[0] * 2,
                        Height = values[0] * 2,
                        Stroke = Brushes.Blue,
                        StrokeThickness = 2,
                        Fill = Brushes.LightBlue,
                        Opacity = 0.7
                    };
                    break;
                case "Ellipse":
                    newShape = new Ellipse
                    {
                        Width = values[0] * 2,
                        Height = values[1] * 2,
                        Stroke = Brushes.Red,
                        StrokeThickness = 2,
                        Fill = Brushes.LightCoral,
                        Opacity = 0.7
                    };
                    break;
                case "Rectangle":
                    newShape = new Rectangle
                    {
                        Width = values[0],
                        Height = values[1],
                        Stroke = Brushes.Green,
                        StrokeThickness = 2,
                        Fill = Brushes.LightGreen,
                        Opacity = 0.7
                    };
                    break;
                case "Triangle":
                    var triangle = new Polygon
                    {
                        Stroke = Brushes.Orange,
                        StrokeThickness = 2,
                        Fill = Brushes.Yellow,
                        Opacity = 0.7,
                        Points = new PointCollection
                                    {
                                        new Point(drawX, drawY + values[1]),
                                        new Point(drawX + values[0] / 2, drawY),
                                        new Point(drawX + values[0], drawY + values[1])
                                    }
                    };
                    newShape = triangle;
                    break;
            }

            if (newShape != null && shape != "Triangle" && shape != "abcxyz2D")
            {
                Canvas.SetLeft(newShape, drawX);
                Canvas.SetTop(newShape, drawY);
            }

            if (newShape != null && canvas2D != null)
            {
                newShape.MouseLeftButtonDown += Shape_MouseLeftButtonDown;
                newShape.MouseRightButtonDown += Shape_MouseRightButtonDownOnShape;
                canvas2D.Children.Add(newShape);
                DrawAxes();
            }
        }

        private void DeleteShapeButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedShape != null && canvas2D != null)
            {
                canvas2D.Children.Remove(selectedShape);
                selectedShape = null;
                DrawAxes();
            }
            else
            {
                MessageBox.Show("Hãy chọn một hình muốn xóa (bấm chuột trái vào hình).");
            }
        }

        private void Shape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedShape = sender as Shape;
            dragStart = e.GetPosition(canvas2D);
            selectedShape?.CaptureMouse();
            e.Handled = true;
        }

        private void Shape_MouseRightButtonDownOnShape(object sender, MouseButtonEventArgs e)
        {
            if (sender is Shape shape && shape != null)
            {
                if (MessageBox.Show("Bạn có muốn xóa hình này không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    canvas2D.Children.Remove(shape);
                    if (selectedShape == shape)
                        selectedShape = null;
                    DrawAxes();
                }
            }
            e.Handled = true;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (selectedShape != null)
                {
                    selectedShape.ReleaseMouseCapture();
                    selectedShape = null;
                    dragStart = null;
                }
                else
                {
                    var pos = e.GetPosition(canvas2D);
                    drawX = (pos.X - offsetX);
                    drawY = (pos.Y - offsetY);
                    if (CoordXBox != null)
                        CoordXBox.Text = drawX.ToString("0.##", CultureInfo.InvariantCulture);
                    if (CoordYBox != null)
                        CoordYBox.Text = drawY.ToString("0.##", CultureInfo.InvariantCulture);
                }
            }
            else if (e.ChangedButton == MouseButton.Middle)
            {
                panStart = e.GetPosition(this);
                canvas2D?.CaptureMouse();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedShape != null && dragStart.HasValue && e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(canvas2D);
                double dx = (pos.X - dragStart.Value.X);
                double dy = (pos.Y - dragStart.Value.Y);
                double left = Canvas.GetLeft(selectedShape);
                double top = Canvas.GetTop(selectedShape);
                Canvas.SetLeft(selectedShape, left + dx);
                Canvas.SetTop(selectedShape, top + dy);
                dragStart = pos;
            }
            else if (panStart.HasValue && e.MiddleButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(this);
                double dx = pos.X - panStart.Value.X;
                double dy = pos.Y - panStart.Value.Y;
                offsetX += dx;
                offsetY += dy;
                panStart = pos;
                UpdateCanvasTransform();
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedShape != null)
            {
                selectedShape.ReleaseMouseCapture();
                selectedShape = null;
                dragStart = null;
            }
            if (panStart.HasValue)
            {
                panStart = null;
                canvas2D?.ReleaseMouseCapture();
            }
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == canvas2D)
            {
                var pos = e.GetPosition(canvas2D);
                MessageBox.Show($"Coordinate: ({(pos.X - offsetX):0.##}, {(pos.Y - offsetY):0.##})");
            }
        }
        private void Canvas2D_Loaded(object sender, RoutedEventArgs e)
        {
            CenterAxes();
        }

        private void UpdateCanvasTransform()
        {
            var matrix = new Matrix();
            matrix.Translate(offsetX, offsetY);
            if (canvas2D != null)
                canvas2D.RenderTransform = new MatrixTransform(matrix);
            DrawAxes();
        }
    }
}