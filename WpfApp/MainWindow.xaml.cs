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

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        private double drawX = 0;
        private double drawY = 0;
        private double drawZ = 0; // For 3D mode
        private readonly List<ShapeContainer> _shapes = new();

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
            if (canvas2D == null || CoordXBox == null || CoordYBox == null) return;
            var pos = e.GetPosition(canvas2D);

            // Chuyển từ canvas sang world
            var worldPos = canvas2D.CanvasToWorld(pos);

            CoordXBox.Text = worldPos.X.ToString("0.##", CultureInfo.InvariantCulture);
            CoordYBox.Text = worldPos.Y.ToString("0.##", CultureInfo.InvariantCulture);
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
                    if (!double.TryParse(tb.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double val) || val <= 0)
                    {
                        MessageBox.Show("Vui lòng nhập số dương hợp lệ cho tất cả trường.");
                        return;
                    }
                    values[i] = val;
                }
            }

            ShapeContainer newShape = null;
            switch (shape)
            {
                case "Circle":
                    newShape = new Circle(canvas2D, new Point(drawX, drawY), values[0], Brushes.Blue, 2, Brushes.LightBlue);
                    break;
                case "Ellipse":
                    newShape = new Eclipse(canvas2D, new Point(drawX, drawY), values[0], values[1], Brushes.Red, 2, Brushes.LightCoral);
                    break;
                case "Rectangle":
                    newShape = new Rectangle(canvas2D, new Point(drawX, drawY), values[0], values[1], Brushes.Green, 2, Brushes.LightGreen);
                    break;
                case "Triangle":
                    var p1 = new Point(drawX, drawY + values[1]);
                    var p2 = new Point(drawX + values[0] / 2, drawY);
                    var p3 = new Point(drawX + values[0], drawY + values[1]);
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

        //private void MoveShapeButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (_shapes.Count == 0)
        //    {
        //        MessageBox.Show("Không có hình nào để di chuyển.");
        //        return;
        //    }
        //    if (canvas2D == null) return;
        //    // Di chuyển hình cuối cùng
        //    var lastShape = _shapes[_shapes.Count - 1];
        //    lastShape.TransformShape(s => TransformationMatrix.Symmetric(s, new Point(-10,10)));
        //    canvas2D.InvalidateVisual();
        //}
    }
}