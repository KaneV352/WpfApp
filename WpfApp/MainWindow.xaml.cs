using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using WpfApp.ThreeDimension;
using WpfApp.ThreeDimension.Models;
using WpfApp.ThreeDimension.Shapes;
using WpfApp.TwoDimension;
using WpfApp.TwoDimension.Models;
using WpfApp.TwoDimension.Samples;
using WpfApp.TwoDimension.Shapes;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        private double drawX = 0;
        private double drawY = 0;
        private double drawZ = 0; // For 3D mode
        private readonly List<ShapeContainer> _shapes = new();

        // Advanced: For click-to-draw
        private List<Point> _pendingPoints = new();
        private int _requiredPoints = 0;
        private string _pendingShape = null;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Console.WriteLine("MainWindow loaded successfully.");
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Mode2DButton != null)
                Mode2DButton.IsChecked = true;
            UpdateShapeComboBox("2D");
            UpdateCoordPanelForMode("2D");
            if (ShapeComboBox != null)
                ShapeComboBox.SelectedIndex = 0;
            if (CoordXBox != null)
                CoordXBox.Text = "0";
            if (CoordYBox != null)
                CoordYBox.Text = "0";
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
            UpdateCoordPanelForMode("2D");
        }

        private void Mode2DButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Mode3DButton == null || Mode3DButton.IsChecked != true)
            {
                if (Mode2DButton != null)
                    Mode2DButton.IsChecked = true;
            }
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
            UpdateCoordPanelForMode("3D");
        }

        private void Mode3DButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Mode2DButton == null || Mode2DButton.IsChecked != true)
            {
                if (Mode3DButton != null)
                    Mode3DButton.IsChecked = true;
            }
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
            // Chọn lại item đầu tiên phù hợp
            foreach (ComboBoxItem item in ShapeComboBox.Items)
            {
                if (item.Visibility == Visibility.Visible)
                {
                    ShapeComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void UpdateCoordPanelForMode(string mode)
        {
            if (CoordPanel == null) return;
            // Xóa Z nếu có
            if (CoordPanel.Children.Count > 4)
                CoordPanel.Children.RemoveRange(4, CoordPanel.Children.Count - 4);

            if (mode == "3D")
            {
                CoordPanel.Children.Add(new TextBlock { Text = "Z:", VerticalAlignment = VerticalAlignment.Center });
                var zBox = new TextBox { Name = "CoordZBox", Width = 50, Margin = new Thickness(5, 0, 0, 0) };
                zBox.TextChanged += CoordBox_TextChanged;
                CoordPanel.Children.Add(zBox);
            }
        }

        private void ShapeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ShapeComboBox == null) return;
            if (ShapeComboBox.SelectedItem is ComboBoxItem item)
            {
                string shape = item.Content?.ToString() ?? string.Empty;
                _pendingPoints.Clear();
                _pendingShape = shape;
                switch (shape)
                {
                    case "Circle":
                        _requiredPoints = 2; // Center, point on circle
                        break;
                    case "Ellipse":
                        _requiredPoints = 3; // Center, point on X axis, point on Y axis
                        break;
                    case "Rectangle":
                        _requiredPoints = 2; // Two opposite corners
                        break;
                    case "Triangle":
                        _requiredPoints = 3; // Three points
                        break;
                    default:
                        _requiredPoints = 0;
                        break;
                }
                if (InputFieldsPanel != null)
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
                        AddInputField("TopLeft X:");
                        AddInputField("TopLeft Y:");
                        AddInputField("BottomRight X:");
                        AddInputField("BottomRight Y:");
                        break;
                    case "Triangle":
                        AddInputField("Point1 X:");
                        AddInputField("Point1 Y:");
                        AddInputField("Point2 X:");
                        AddInputField("Point2 Y:");
                        AddInputField("Point3 X:");
                        AddInputField("Point3 Y:");
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
            if (InputFieldsPanel == null) return;
            var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 0) };
            panel.Children.Add(new TextBlock { Text = label, Width = 80, VerticalAlignment = VerticalAlignment.Center });
            panel.Children.Add(new TextBox { Width = 100 });
            InputFieldsPanel.Children.Add(panel);
        }

        private void CoordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CoordXBox != null && double.TryParse(CoordXBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double x))
                drawX = x;
            if (CoordYBox != null && double.TryParse(CoordYBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double y))
                drawY = y;
            // Nếu có Z
            var zBox = FindName("CoordZBox") as TextBox;
            if (zBox != null && double.TryParse(zBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double z))
                drawZ = z;
        }

        private void Canvas2D_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (canvas2D == null) return;
            var pos = e.GetPosition(canvas2D);
            var worldPos = canvas2D.CanvasToWorld(pos);

            // Advanced: click-to-draw mode
            if (_requiredPoints > 0 && !string.IsNullOrEmpty(_pendingShape))
            {
                _pendingPoints.Add(worldPos);
                // Optionally, show a temporary point
                canvas2D.AddPoint(worldPos, Brushes.Gray, 3);

                if (_pendingPoints.Count == _requiredPoints)
                {
                    DrawShapeFromPoints();
                    _pendingPoints.Clear();
                }
                return;
            }

            // Default: update coordinate input boxes
            if (CoordXBox != null)
                CoordXBox.Text = worldPos.X.ToString("0.##", CultureInfo.InvariantCulture);
            if (CoordYBox != null)
                CoordYBox.Text = worldPos.Y.ToString("0.##", CultureInfo.InvariantCulture);
        }

        private void DrawShapeFromPoints()
        {
            ShapeContainer newShape = null;
            switch (_pendingShape)
            {
                case "Circle":
                    var center = _pendingPoints[0];
                    var radius = Math.Sqrt(Math.Pow(_pendingPoints[1].X - center.X, 2) + Math.Pow(_pendingPoints[1].Y - center.Y, 2));
                    if (radius <= 0)
                    {
                        MessageBox.Show("Bán kính phải > 0.");
                        return;
                    }
                    newShape = new Circle(canvas2D, center, radius, Brushes.Blue, 2, Brushes.LightBlue);
                    break;
                case "Ellipse":
                    var c = _pendingPoints[0];
                    var xPoint = _pendingPoints[1];
                    var yPoint = _pendingPoints[2];
                    var rx = Math.Abs(xPoint.X - c.X);
                    var ry = Math.Abs(yPoint.Y - c.Y);
                    if (rx <= 0 || ry <= 0)
                    {
                        MessageBox.Show("Bán trục X và Y phải > 0.");
                        return;
                    }
                    newShape = new Eclipse(canvas2D, c, rx, ry, Brushes.Red, 2, Brushes.LightCoral);
                    break;
                case "Rectangle":
                    var topLeft = _pendingPoints[0];
                    var bottomRight = _pendingPoints[1];
                    newShape = new Rectangle(canvas2D, topLeft, 0, bottomRight, Brushes.Green, 2, Brushes.LightGreen);
                    break;
                case "Triangle":
                    var p1 = _pendingPoints[0];
                    var p2 = _pendingPoints[1];
                    var p3 = _pendingPoints[2];
                    newShape = new WpfApp.TwoDimension.Shapes.Triangle(canvas2D, p1, p2, p3, Brushes.Orange, 2, Brushes.Yellow);
                    break;
            }

            if (newShape != null)
            {
                _shapes.Add(newShape);
                foreach (var segment in newShape.Segments)
                {
                    if (segment is FillSegment fillSegment)
                        canvas2D.AddFill(fillSegment);
                    else if (segment is WpfApp.TwoDimension.Models.LineSegment lineSegment)
                        canvas2D.AddLine(lineSegment.WorldStart, lineSegment.WorldEnd, lineSegment.Stroke, lineSegment.Thickness);
                    else if (segment is PointSegment pointSegment)
                        canvas2D.AddPoint(pointSegment.WorldPoint, pointSegment.Fill, pointSegment.Size);
                }
            }
        }

        private void Canvas2D_Loaded(object sender, RoutedEventArgs e)
        {
            // Vẽ trục tọa độ OX, OY
            if (canvas2D == null) return;

            double width = canvas2D.ActualWidth;
            double height = canvas2D.ActualHeight;

            // Trục X (ngang)
            canvas2D.AddLine(
                new Point(0, height / 2),
                new Point(width, height / 2),
                Brushes.Gray, 1);

            // Trục Y (dọc)
            canvas2D.AddLine(
                new Point(width / 2, 0),
                new Point(width / 2, height),
                Brushes.Gray, 1);
        }

        private void AddShape_Click(object sender, RoutedEventArgs e)
        {
            if (canvas2D == null || canvas2D.Visibility != Visibility.Visible)
            {
                MessageBox.Show("3D mode chỉ là placeholder.");
                return;
            }

            if (ShapeComboBox == null || ShapeComboBox.SelectedItem is not ComboBoxItem item)
                return;

            string shape = item.Content?.ToString() ?? string.Empty;
            if (InputFieldsPanel == null) return;
            double[] values = new double[InputFieldsPanel.Children.Count];
            for (int i = 0; i < InputFieldsPanel.Children.Count; i++)
            {
                if (InputFieldsPanel.Children[i] is StackPanel panel &&
                    panel.Children[1] is TextBox tb)
                {
                    if (!double.TryParse(tb.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
                    {
                        MessageBox.Show("Vui lòng nhập số hợp lệ cho tất cả trường.");
                        return;
                    }
                    values[i] = val;
                }
            }

            ShapeContainer newShape = null;
            switch (shape)
            {
                case "Circle":
                    if (values.Length < 1 || values[0] <= 0)
                    {
                        MessageBox.Show("Bán kính phải > 0.");
                        return;
                    }
                    newShape = new Circle(canvas2D, new Point(drawX, drawY), values[0], Brushes.Blue, 2, Brushes.LightBlue);
                    break;
                case "Ellipse":
                    if (values.Length < 2 || values[0] <= 0 || values[1] <= 0)
                    {
                        MessageBox.Show("Bán trục X và Y phải > 0.");
                        return;
                    }
                    newShape = new Eclipse(canvas2D, new Point(drawX, drawY), values[0], values[1], Brushes.Red, 2, Brushes.LightCoral);
                    break;
                case "Rectangle":
                    if (values.Length < 4)
                    {
                        MessageBox.Show("Vui lòng nhập đủ 4 giá trị cho hình chữ nhật.");
                        return;
                    }
                    var topLeft = new Point(values[0], values[1]);
                    var bottomRight = new Point(values[2], values[3]);
                    newShape = new Rectangle(canvas2D, topLeft, 0, bottomRight, Brushes.Green, 2, Brushes.LightGreen);
                    break;
                case "Triangle":
                    if (values.Length < 6)
                    {
                        MessageBox.Show("Vui lòng nhập đủ 6 giá trị cho hình tam giác.");
                        return;
                    }
                    var p1 = new Point(values[0], values[1]);
                    var p2 = new Point(values[2], values[3]);
                    var p3 = new Point(values[4], values[5]);
                    newShape = new WpfApp.TwoDimension.Shapes.Triangle(canvas2D, p1, p2, p3, Brushes.Orange, 2, Brushes.Yellow);
                    break;
                // 3D shapes: chỉ placeholder
                case "Sphere":
                case "Cube":
                case "Pyramid":
                    MessageBox.Show("Chế độ 3D chỉ là placeholder.");
                    return;
            }

            if (newShape != null)
            {
                _shapes.Add(newShape);
                foreach (var segment in newShape.Segments)
                {
                    if (segment is FillSegment fillSegment)
                        canvas2D.AddFill(fillSegment);
                    else if (segment is WpfApp.TwoDimension.Models.LineSegment lineSegment)
                        canvas2D.AddLine(lineSegment.WorldStart, lineSegment.WorldEnd, lineSegment.Stroke, lineSegment.Thickness);
                    else if (segment is PointSegment pointSegment)
                        canvas2D.AddPoint(pointSegment.WorldPoint, pointSegment.Fill, pointSegment.Size);
                }
            }
        }

        private void DeleteShapeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_shapes.Count > 0)
            {
                _shapes.RemoveAt(_shapes.Count - 1);
                if (canvas2D != null)
                    canvas2D.ClearAll();
                foreach (var shape in _shapes)
                {
                    foreach (var segment in shape.Segments)
                    {
                        if (segment is FillSegment fillSegment)
                            canvas2D.AddFill(fillSegment);
                        else if (segment is WpfApp.TwoDimension.Models.LineSegment lineSegment)
                            canvas2D.AddLine(lineSegment.WorldStart, lineSegment.WorldEnd, lineSegment.Stroke, lineSegment.Thickness);
                        else if (segment is PointSegment pointSegment)
                            canvas2D.AddPoint(pointSegment.WorldPoint, pointSegment.Fill, pointSegment.Size);
                    }
                }
            }
            else
            {
                MessageBox.Show("Không có hình nào để xóa.");
            }
        }
    }
}