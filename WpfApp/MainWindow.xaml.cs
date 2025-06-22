using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp.TwoDimension;
using WpfApp.TwoDimension.Models;
using WpfApp.TwoDimension.Shapes;
using WpfApp.ThreeDimension;
using WpfApp.ThreeDimension.Models;
using WpfApp.ThreeDimension.Shapes;
using System.Windows.Media.Media3D;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        private double drawX = 0;
        private double drawY = 0;
        private double drawZ = 0;
        private readonly List<ShapeContainer> _shapes = new();
        private readonly List<object> _shapes3D = new();
        private List<Point> _pendingPoints = new();
        private int _requiredPoints = 0;
        private string _pendingShape = null;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
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
            if (CoordPanel != null)
                CoordPanel.Visibility = Visibility.Visible;
        }

        private void Mode2DButton_Checked(object sender, RoutedEventArgs e)
        {
            if (Mode3DButton != null)
                Mode3DButton.IsChecked = false;
            if (canvas2D != null)
                canvas2D.Visibility = Visibility.Visible;
            if (canvas3D != null)
                canvas3D.Visibility = Visibility.Collapsed;
            UpdateShapeComboBox("2D");
            UpdateCoordPanelForMode("2D");
            if (CoordPanel != null)
                CoordPanel.Visibility = Visibility.Visible; 
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
            if (canvas3D != null)
                canvas3D.Visibility = Visibility.Visible;
            UpdateShapeComboBox("3D");
            UpdateCoordPanelForMode("3D");
            if (CoordPanel != null)
                CoordPanel.Visibility = Visibility.Collapsed; 
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
            if (CoordPanel.Children.Count > 4)
                CoordPanel.Children.RemoveRange(4, CoordPanel.Children.Count - 4);

            if (mode == "3D")
            {
                // CoordPanel.Children.Add(new TextBlock { Text = "Z:", VerticalAlignment = VerticalAlignment.Center });
                // var zBox = new TextBox { Name = "CoordZBox", Width = 50, Margin = new Thickness(5, 0, 0, 0) };
                // zBox.TextChanged += CoordBox_TextChanged;
                // CoordPanel.Children.Add(zBox);
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
                        _requiredPoints = 2;
                        break;
                    case "Ellipse":
                        _requiredPoints = 3;
                        break;
                    case "Rectangle":
                        _requiredPoints = 2;
                        break;
                    case "Triangle":
                        _requiredPoints = 3;
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
                        AddInputField("Center X:");
                        AddInputField("Center Y:");
                        AddInputField("Center Z:");
                        AddInputField("Radius:");
                        break;
                    case "Cube":
                        AddInputField("BottomLeftBack X:");
                        AddInputField("BottomLeftBack Y:");
                        AddInputField("BottomLeftBack Z:");
                        AddInputField("Side:");
                        break;
                    case "Cuboid":
                        AddInputField("Origin X:");
                        AddInputField("Origin Y:");
                        AddInputField("Origin Z:");
                        AddInputField("Width:");
                        AddInputField("Height:");
                        AddInputField("Depth:");
                        break;
                    case "Pyramid":
                        AddInputField("Base X:");
                        AddInputField("Base Y:");
                        AddInputField("Base Z:");
                        AddInputField("Base Width:");
                        AddInputField("Base Length:");
                        AddInputField("Height:");
                        break;
                    case "Cylinder":
                        AddInputField("Base X:");
                        AddInputField("Base Y:");
                        AddInputField("Base Z:");
                        AddInputField("Radius:");
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
            var zBox = FindName("CoordZBox") as TextBox;
            if (zBox != null && double.TryParse(zBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double z))
                drawZ = z;
        }

        private void Canvas2D_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (canvas2D == null) return;
            var pos = e.GetPosition(canvas2D);
            var worldPos = canvas2D.CanvasToWorld(pos);
            if (_requiredPoints > 0 && !string.IsNullOrEmpty(_pendingShape))
            {
                _pendingPoints.Add(worldPos);
                canvas2D.AddPoint(worldPos, Brushes.Gray, 3);

                if (_pendingPoints.Count == _requiredPoints)
                {
                    DrawShapeFromPoints();
                    _pendingPoints.Clear();
                }
                return;
            }
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
                    newShape = new Rectangle(canvas2D, topLeft, bottomRight, Brushes.Green, 2, Brushes.LightGreen);
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

        private void Canvas3D_Loaded(object sender, RoutedEventArgs e)
        {
            if (canvas3D == null) return;

            canvas3D.ClearContent();
            // x
            canvas3D.AddLine(new Point3D(0, 0, 0), new Point3D(10, 0, 0), Colors.Red, 0.1);
            // y
            canvas3D.AddLine(new Point3D(0, 0, 0), new Point3D(0, 10, 0), Colors.Green, 0.1);
            // z
            canvas3D.AddLine(new Point3D(0, 0, 0), new Point3D(0, 0, 10), Colors.Blue, 0.1);
            double tickSize = 0.12;
            int tickCount = 3;
            for (int i = 1; i <= tickCount; i++)
            {
                canvas3D.AddLine(new Point3D(i, -tickSize, 0), new Point3D(i, tickSize, 0), Colors.Red, 0.04);
                canvas3D.AddLine(new Point3D(-tickSize, i, 0), new Point3D(tickSize, i, 0), Colors.Green, 0.04);
                canvas3D.AddLine(new Point3D(0, -tickSize, i), new Point3D(0, tickSize, i), Colors.Blue, 0.04);
            }
        }

        private void AddShape_Click(object sender, RoutedEventArgs e)
        {
            if (canvas2D != null && canvas2D.Visibility == Visibility.Visible)
            {
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
                        newShape = new Rectangle(canvas2D, topLeft, bottomRight, Brushes.Green, 2, Brushes.LightGreen);
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
            else if (canvas3D != null && canvas3D.Visibility == Visibility.Visible)
            {
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

                object newShape3D = null;
                switch (shape)
                {
                    case "Cylinder":
                        if (values.Length < 5)
                        {
                            MessageBox.Show("Vui lòng nhập đủ thông tin cho Cylinder.");
                            return;
                        }
                        var baseCenter = new Point3D(values[0], values[1], values[2]);
                        var radius = values[3];
                        var height = values[4];
                        if (radius <= 0 || height <= 0)
                        {
                            MessageBox.Show("Bán kính và chiều cao phải > 0.");
                            return;
                        }
                        newShape3D = new Cylinder(canvas3D, baseCenter, radius, height, Colors.Orange, 0.08, 32);
                        break;
                    case "Sphere":
                        if (values.Length < 4)
                        {
                            MessageBox.Show("Vui lòng nhập đủ thông tin cho Sphere.");
                            return;
                        }
                        var center = new Point3D(values[0], values[1], values[2]);
                        var r = values[3];
                        if (r <= 0)
                        {
                            MessageBox.Show("Bán kính phải > 0.");
                            return;
                        }
                        newShape3D = new Sphere(canvas3D, center, r, Colors.Blue, 0.08, 32);
                        break;
                    case "Cube":
                        if (values.Length < 4)
                        {
                            MessageBox.Show("Vui lòng nhập đủ thông tin cho Cube.");
                            return;
                        }
                        var cubeOrigin = new Point3D(values[0], values[1], values[2]);
                        var side = values[3];
                        if (side <= 0)
                        {
                            MessageBox.Show("Cạnh phải > 0.");
                            return;
                        }
                        newShape3D = new Cube(canvas3D, cubeOrigin, side, Colors.Green, 0.08);
                        break;
                    case "Cuboid":
                        if (values.Length < 6)
                        {
                            MessageBox.Show("Vui lòng nhập đủ thông tin cho Cuboid.");
                            return;
                        }
                        var cuboidOrigin = new Point3D(values[0], values[1], values[2]);
                        var width = values[3];
                        var heightCuboid = values[4];
                        var depth = values[5];
                        if (width <= 0 || heightCuboid <= 0 || depth <= 0)
                        {
                            MessageBox.Show("Kích thước phải > 0.");
                            return;
                        }
                        newShape3D = new Cuboid(canvas3D, cuboidOrigin, width, heightCuboid, depth, Colors.Purple, 0.08);
                        break;

                    case "Pyramid":
                        if (values.Length < 6)
                        {
                            MessageBox.Show("Vui lòng nhập đủ thông tin cho Pyramid.");
                            return;
                        }
                        var baseOrigin = new Point3D(values[0], values[1], values[2]);
                        var baseWidth = values[3];
                        var baseLength = values[4];
                        var pyramidHeight = values[5];
                        if (baseWidth <= 0 || baseLength <= 0 || pyramidHeight <= 0)
                        {
                            MessageBox.Show("Kích thước phải > 0.");
                            return;
                        }
                        newShape3D = new Pyramid(canvas3D, baseOrigin, baseWidth, baseLength, pyramidHeight, Colors.Red, 0.08);
                        break;
                }

                if (newShape3D != null)
                {
                    _shapes3D.Add(newShape3D);
                }
            }
        }

        private void DeleteShapeButton_Click(object sender, RoutedEventArgs e)
        {
            if (canvas2D != null && canvas2D.Visibility == Visibility.Visible)
            {
                if (_shapes.Count > 0)
                {
                    _shapes.RemoveAt(_shapes.Count - 1);
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
            else if (canvas3D != null && canvas3D.Visibility == Visibility.Visible)
            {
                if (_shapes3D.Count > 0)
                {
                    _shapes3D.RemoveAt(_shapes3D.Count - 1);
                    canvas3D.ClearContent();
                    foreach (var shape3D in _shapes3D)
                    {
                        var method = shape3D.GetType().GetMethod("Draw");
                        if (method != null)
                        {
                            method.Invoke(shape3D, null);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Không có hình 3D nào để xóa.");
                }
            }
        }
    }
}