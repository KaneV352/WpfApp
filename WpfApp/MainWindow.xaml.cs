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
using WpfApp.TwoDimension.Animations;
using WpfApp.TwoDimension.Models;
using WpfApp.TwoDimension.Samples;
using WpfApp.TwoDimension.Shapes;
using System.Linq; // Added for .Average()
using WpfApp.Animation;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        private double drawX = 0;
        private double drawY = 0;
        private double drawZ = 0;
        private readonly List<ShapeContainer> _shapes = new();
        private readonly List<ShapeContainer3D> _shapes3D = new();
        private List<Point> _pendingPoints = new();
        private int _requiredPoints = 0;
        private string _pendingShape = null;
        private ShapeContainer? _selectedShape = null;
        private bool _isSelectionMode = false; // New field to track selection mode

        private Animator _animator = new Animator(); // Instantiate the Animator

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            
            _animator.Start(); // Start the animator when the window loads
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
            if (InstructionTextBlock != null)
                InstructionTextBlock.Text = "Chọn hình, nhập thông số, sau đó nhấn 'Thêm hình'.";
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
            if (InstructionTextBlock != null)
                InstructionTextBlock.Text = "Chọn hình 2D, nhập thông số, sau đó nhấn 'Thêm hình'.";
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
            if (InstructionTextBlock != null)
                InstructionTextBlock.Text = "Chọn hình 3D, nhập thông số, sau đó nhấn 'Thêm hình'.";
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
                string? tag = item.Tag?.ToString(); // Changed to nullable string
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
                // Add Z input if needed
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

                // Update instructions
                if (InstructionTextBlock != null)
                {
                    switch (shape)
                    {
                        case "Rectangle":
                            InstructionTextBlock.Text = "Nhập tọa độ cho 2 điểm góc của hình chữ nhật.";
                            break;
                        case "Triangle":
                            InstructionTextBlock.Text = "Nhập tọa độ cho 3 điểm của hình tam giác.";
                            break;
                        case "Circle":
                            InstructionTextBlock.Text = "Nhập bán kính cho hình tròn.";
                            break;
                        case "Ellipse":
                            InstructionTextBlock.Text = "Nhập bán trục X và Y cho ellipse.";
                            break;
                        default:
                            InstructionTextBlock.Text = "Chọn hình, nhập thông số, sau đó nhấn 'Thêm hình'.";
                            break;
                    }
                }

                switch (shape)
                {
                    case "Circle":
                        AddInputField("Bán kính:");
                        break;
                    case "Ellipse":
                        AddInputField("Bán trục X:");
                        AddInputField("Bán trục Y:");
                        break;
                    case "Rectangle":
                        AddXYInputField("Góc trên trái (X, Y):");
                        AddXYInputField("Góc dưới phải (X, Y):");
                        break;
                    case "Triangle":
                        AddXYInputField("Điểm 1 (X, Y):");
                        AddXYInputField("Điểm 2 (X, Y):");
                        AddXYInputField("Điểm 3 (X, Y):");
                        break;
                    case "Sphere":
                        AddInputField("Tâm X:");
                        AddInputField("Tâm Y:");
                        AddInputField("Tâm Z:");
                        AddInputField("Bán kính:");
                        break;
                    case "Cube":
                        AddInputField("Góc X:");
                        AddInputField("Góc Y:");
                        AddInputField("Góc Z:");
                        AddInputField("Cạnh:");
                        break;
                    case "Cuboid":
                        AddInputField("Gốc X:");
                        AddInputField("Gốc Y:");
                        AddInputField("Gốc Z:");
                        AddInputField("Rộng:");
                        AddInputField("Cao:");
                        AddInputField("Sâu:");
                        break;
                    case "Pyramid":
                        AddInputField("Đáy X:");
                        AddInputField("Đáy Y:");
                        AddInputField("Đáy Z:");
                        AddInputField("Rộng đáy:");
                        AddInputField("Dài đáy:");
                        AddInputField("Cao:");
                        break;
                    case "Cylinder":
                        AddInputField("Tâm X:");
                        AddInputField("Tâm Y:");
                        AddInputField("Tâm Z:");
                        AddInputField("Bán kính:");
                        AddInputField("Cao:");
                        break;
                }
            }
        }

        private void AddInputField(string label)
        {
            if (InputFieldsPanel == null) return;
            var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 0) };
            panel.Children.Add(new TextBlock { Text = label, Width = 90, VerticalAlignment = VerticalAlignment.Center });
            panel.Children.Add(new TextBox { Width = 80 });
            InputFieldsPanel.Children.Add(panel);
        }

        // New: Add XY input field for grouped X/Y entry
        private void AddXYInputField(string label)
        {
            if (InputFieldsPanel == null) return;
            var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 0) };
            panel.Children.Add(new TextBlock { Text = label, Width = 130, VerticalAlignment = VerticalAlignment.Center });
            var xBox = new TextBox { Width = 50, Margin = new Thickness(0, 0, 5, 0) };
            var yBox = new TextBox { Width = 50 };
            panel.Children.Add(xBox);
            panel.Children.Add(new TextBlock { Text = ",", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2, 0, 2, 0) });
            panel.Children.Add(yBox);
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

        private void _shapeSelectedByClick(Point clickPoint)
        {
            const double threshold = 5.0; // bán kính tối đa tính là "gần"

            ShapeContainer? found = null;
            foreach (var shape in _shapes)
            {
                foreach (var seg in shape.Segments)
                {
                    foreach (var p in seg.WorldPoints)
                    {
                        if ((p - clickPoint).Length <= threshold)
                        {
                            found = shape;
                            break;
                        }
                    }
                    if (found != null) break;
                }
                if (found != null) break;
            }

            _selectedShape = found;
            _isSelectionMode = (_selectedShape != null); // Update selection mode
            RedrawAllShapes();
        }

        private void Canvas2D_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (canvas2D == null) return;
            var pos = e.GetPosition(canvas2D);
            var worldPos = canvas2D.CanvasToWorld(pos);
            // Thử chọn 1 shape gần vị trí click
            var clickedPoint = worldPos;
            _shapeSelectedByClick(clickedPoint);

            // Advanced: click-to-draw mode - Only allow if not in selection mode
            if (!_isSelectionMode && _requiredPoints > 0 && !string.IsNullOrEmpty(_pendingShape))
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
            ShapeContainer? newShape = null;
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
            }
        }

        private void Canvas2D_Loaded(object sender, RoutedEventArgs e)
        {
            if (canvas2D == null) return;

            canvas2D.ClearAll();
            _shapes.Clear();
            _pendingPoints.Clear();
            _pendingShape = null;
            _requiredPoints = 0;


            // Animation example
        }

        private void Canvas3D_Loaded(object sender, RoutedEventArgs e)
        {
            if (canvas3D == null) return;

            canvas3D.ClearAll();
            _shapes3D.Clear();
            _pendingPoints.Clear();
            _pendingShape = null;
            _requiredPoints = 0;
            drawX = 0;
            drawY = 0;
            drawZ = 0;
            if (CoordXBox != null)
                CoordXBox.Text = "0";
            if (CoordYBox != null)
                CoordYBox.Text = "0";
            var zBox = FindName("CoordZBox") as TextBox;
            if (zBox != null)
                zBox.Text = "0";
        }

        private void AddShape_Click(object sender, RoutedEventArgs e)
        {
            if (canvas2D != null && canvas2D.Visibility == Visibility.Visible)
            {
                if (ShapeComboBox == null || ShapeComboBox.SelectedItem is not ComboBoxItem item)
                    return;

                string shape = item.Content?.ToString() ?? string.Empty;
                if (InputFieldsPanel == null) return;

                ShapeContainer newShape = null;

                if (shape == "Rectangle" || shape == "Triangle")
                {
                    var points = new List<Point>();
                    foreach (StackPanel panel in InputFieldsPanel.Children)
                    {
                        if (panel.Children.Count >= 4 &&
                            panel.Children[1] is TextBox xBox &&
                            panel.Children[3] is TextBox yBox)
                        {
                            if (string.IsNullOrWhiteSpace(xBox.Text) || string.IsNullOrWhiteSpace(yBox.Text))
                            {
                                MessageBox.Show("Vui lòng nhập đầy đủ tọa độ cho tất cả các điểm.");
                                return;
                            }
                            if (!double.TryParse(xBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double x) ||
                                !double.TryParse(yBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double y))
                            {
                                MessageBox.Show("Tọa độ phải là số hợp lệ.");
                                return;
                            }
                            points.Add(new Point(x, y));
                        }
                    }
                    if (shape == "Rectangle")
                    {
                        if (points.Count < 2)
                        {
                            MessageBox.Show("Vui lòng nhập đủ 2 điểm cho hình chữ nhật.");
                            return;
                        }
                        newShape = new Rectangle(canvas2D, points[0], points[1], Brushes.Green, 2, Brushes.LightGreen);
                    }
                    else if (shape == "Triangle")
                    {
                        if (points.Count < 3)
                        {
                            MessageBox.Show("Vui lòng nhập đủ 3 điểm cho hình tam giác.");
                            return;
                        }
                        newShape = new WpfApp.TwoDimension.Shapes.Triangle(canvas2D, points[0], points[1], points[2], Brushes.Orange, 2, Brushes.Yellow);
                    }
                }
                else
                {
                    double[] values = new double[InputFieldsPanel.Children.Count];
                    for (int i = 0; i < InputFieldsPanel.Children.Count; i++)
                    {
                        if (InputFieldsPanel.Children[i] is StackPanel panel &&
                            panel.Children[1] is TextBox tb)
                        {
                            if (string.IsNullOrWhiteSpace(tb.Text))
                            {
                                MessageBox.Show("Vui lòng nhập đầy đủ thông số.");
                                return;
                            }
                            if (!double.TryParse(tb.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
                            {
                                MessageBox.Show("Giá trị phải là số hợp lệ.");
                                return;
                            }
                            values[i] = val;
                        }
                    }

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
                    }
                }

                if (newShape != null)
                {
                    _shapes.Add(newShape);
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
                        if (string.IsNullOrWhiteSpace(tb.Text))
                        {
                            MessageBox.Show("Vui lòng nhập đầy đủ thông số.");
                            return;
                        }
                        if (!double.TryParse(tb.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
                        {
                            MessageBox.Show("Giá trị phải là số hợp lệ.");
                            return;
                        }
                        values[i] = val;
                    }
                }

                ShapeContainer3D newShape3D = null;
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
                if (_selectedShape != null && _shapes.Contains(_selectedShape))
                {
                    _shapes.Remove(_selectedShape);
                    _selectedShape = null; // Bỏ chọn sau khi xóa
                    _isSelectionMode = false; // Exit selection mode
                }
                else if (_shapes.Count > 0)
                {
                    // Nếu không có shape được chọn, vẫn xóa shape cuối cùng
                    _shapes.RemoveAt(_shapes.Count - 1);
                }
                else
                {
                    MessageBox.Show("Không có hình nào để xóa.");
                    return;
                }
            }
            else if (canvas3D != null && canvas3D.Visibility == Visibility.Visible)
            {
                if (_shapes3D.Count > 0)
                {
                    var deleteShape3D = _shapes3D[^1]; // Lấy hình 3D cuối cùng
                    foreach (var segment3D in deleteShape3D.Segments)
                    {
                        canvas3D.DeleteSegment(segment3D);
                    }
                    _animator.RemoveAnimationsForShape(deleteShape3D);
                    _shapes3D.RemoveAt(_shapes3D.Count - 1);
                }
                else
                {
                    MessageBox.Show("Không có hình 3D nào để xóa.");
                }
            }
            RedrawAllShapes(); // Vẽ lại toàn bộ
        }
        private ShapeContainer? GetLastShape()
        {
            return _shapes.Count > 0 ? _shapes[^1] : null;
        }

        private async void TranslateLeft_Click(object sender, RoutedEventArgs e)
        {
            var shape = _selectedShape ?? GetLastShape(); // Use selected shape, or last one if none selected
            if (shape != null)
                await ShapeAnimation.AnimateTranslate(shape, new Vector(-50, 0));
            RedrawAllShapes();
        }

        private async void TranslateRight_Click(object sender, RoutedEventArgs e)
        {
            var shape = _selectedShape ?? GetLastShape();
            if (shape != null)
                await ShapeAnimation.AnimateTranslate(shape, new Vector(50, 0));
            RedrawAllShapes();
        }

        private async void TranslateUp_Click(object sender, RoutedEventArgs e)
        {
            var shape = _selectedShape ?? GetLastShape();
            if (shape != null)
                await ShapeAnimation.AnimateTranslate(shape, new Vector(0, 50));
            RedrawAllShapes(); // Added redraw to this method
        }

        private async void TranslateDown_Click(object sender, RoutedEventArgs e)
        {
            var shape = _selectedShape ?? GetLastShape();
            if (shape != null)
                await ShapeAnimation.AnimateTranslate(shape, new Vector(0, -50));
            RedrawAllShapes();

        }

        private async void RotateOrigin_Click(object sender, RoutedEventArgs e)
        {
            var shape = _selectedShape ?? GetLastShape();
            if (shape == null) return;

            var allPoints = shape.Segments.SelectMany(s => s.WorldPoints).ToList();
            if (allPoints.Count == 0) return;

            double centerX = allPoints.Average(p => p.X);
            double centerY = allPoints.Average(p => p.Y);
            Point center = new Point(centerX, centerY);

            await ShapeAnimation.AnimateRotate(shape, center, 360);
            RedrawAllShapes();
        }

        private async void RotateAround_Click(object sender, RoutedEventArgs e)
        {
            var shape = _selectedShape ?? GetLastShape();
            if (shape != null)
                await ShapeAnimation.AnimateRotate(shape, new Point(0, 0), 360);
            RedrawAllShapes();
        }

        private async void ScaleUp_Click(object sender, RoutedEventArgs e)
        {
            var shape = _selectedShape ?? GetLastShape();
            if (shape == null) return;

            Point center = GetShapeCenter(shape);
            await ShapeAnimation.AnimateScale(shape, center, 1.2, 1.2);
            RedrawAllShapes();
        }

        private async void ScaleDown_Click(object sender, RoutedEventArgs e)
        {
            var shape = _selectedShape ?? GetLastShape();
            if (shape == null) return;

            Point center = GetShapeCenter(shape);
            await ShapeAnimation.AnimateScale(shape, center, 0.8, 0.8);
            RedrawAllShapes();

        }
        private async void SymmetricButton_Click(object sender, RoutedEventArgs e)
        {
            if (_shapes.Count == 0) return;

            ShapeContainer shape = _selectedShape ?? _shapes[0]; // Use selected shape, or first one if none selected
            Point symmetricPoint = new Point(0, 0);
            await ShapeAnimation.AnimateSymmetric(shape, symmetricPoint);
            RedrawAllShapes();
        }

        private Point GetShapeCenter(ShapeContainer shape)
        {
            var allPoints = shape.Segments.SelectMany(s => s.WorldPoints).ToList();
            double centerX = allPoints.Average(p => p.X);
            double centerY = allPoints.Average(p => p.Y);
            return new Point(centerX, centerY);
        }

        private void RedrawAllShapes()
        {
            if (canvas2D == null)
                return;

            canvas2D.ClearAll();

            foreach (var shape in _shapes)
            {
                bool isSelected = shape == _selectedShape;

                foreach (var segment in shape.Segments)
                {
                    if (segment is FillSegment fillSegment)
                        canvas2D.AddFill(fillSegment);
                    else if (segment is WpfApp.TwoDimension.Models.LineSegment lineSegment)
                    {
                        var stroke = isSelected ? Brushes.Magenta : lineSegment.Stroke;
                        var thickness = isSelected ? lineSegment.Thickness + 1.5 : lineSegment.Thickness;
                        canvas2D.AddLine(lineSegment.WorldStart, lineSegment.WorldEnd, stroke, thickness);
                    }
                    else if (segment is PointSegment pointSegment)
                    {
                        var fill = isSelected ? Brushes.Magenta : pointSegment.Fill;
                        var size = isSelected ? pointSegment.Size + 1 : pointSegment.Size;
                        canvas2D.AddPoint(pointSegment.WorldPoint, fill, size);
                    }
                }
            }
        }
    }
}