using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfApp.Models;
using LineSegment = WpfApp.Models.LineSegment;

public class CartesianCanvas : Canvas
{
    // Origin in canvas coordinates (default center)
    public static readonly DependencyProperty OriginProperty =
        DependencyProperty.Register(
            nameof(Origin),
            typeof(Point),
            typeof(CartesianCanvas),
            new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsRender)
        );

    // Scale: 5 pixels = 1 unit
    public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
        nameof(Scale), typeof(double), typeof(CartesianCanvas),
        new FrameworkPropertyMetadata(5.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public Point Origin
    {
        get => (Point)GetValue(OriginProperty);
        set => SetValue(OriginProperty, value);
    }

    public double Scale
    {
        get => (double)GetValue(ScaleProperty);
        set => SetValue(ScaleProperty, value);
    }

    private readonly List<PointSegment> _points = new List<PointSegment>();
    private readonly List<LineSegment> _lines = new List<LineSegment>();
    private readonly List<ShapeSegment> _fillSegments = new List<ShapeSegment>();

    public CartesianCanvas()
    {
        Loaded += (s, e) => CenterOrigin();
    }
    
    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);
        DrawAxes(dc);
        DrawLines(dc);
        DrawPoints(dc);
        DrawShapes(dc);
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        CenterOrigin();
        InvalidateVisual();
    }

    private void CenterOrigin()
    {
        Origin = new Point(ActualWidth / 2, ActualHeight / 2);
    }

    public PointSegment AddPoint(Point worldPoint, Brush color, double size = 2)
    {
        var point = new PointSegment(worldPoint, color, size);
        point.PointsChanged += OnSegmentPointsChanged;
        _points.Add(point);
        InvalidateVisual();
        return point;
    }

    // Add a line segment and listen for changes
    public LineSegment AddLine(Point start, Point end, Brush stroke, double thickness = 1)
    {
        var line = new LineSegment(start, end, stroke, thickness);
        line.PointsChanged += OnSegmentPointsChanged;
        _lines.Add(line);
        InvalidateVisual();
        return line;
    }

    public ShapeSegment AddFill(FillSegment fillSegment)
    {
        fillSegment.PointsChanged += OnSegmentPointsChanged;
        _fillSegments.Add(fillSegment);
        InvalidateVisual();
        return fillSegment;
    }

    public void ClearAll()
    {
        // Unsubscribe from events
        foreach (var point in _points)
        {
            point.PointsChanged -= OnSegmentPointsChanged;
        }
        foreach (var line in _lines)
        {
            line.PointsChanged -= OnSegmentPointsChanged;
        }
        
        _points.Clear();
        _lines.Clear();
        _fillSegments.Clear();
        Children.Clear();
        InvalidateVisual();
    }
    
    private void OnSegmentPointsChanged(object? sender, EventArgs e)
    {
        InvalidateVisual();
    }
    
    private void DrawPoints(DrawingContext dc)
    {
        foreach (var point in _points)
        {
            var center = WorldToCanvas(point.WorldPoint);
            DrawPoint(dc, (int)center.X, (int)center.Y, point.Fill, (int)(point.Size / 2));
        }
    }

    private void DrawLines(DrawingContext dc)
    {
        foreach (var line in _lines)
        {
            DrawMidpointLine(dc, line.WorldStart, line.WorldEnd, line.Stroke, line.Thickness);
        }
    }
    
    private void DrawShapes(DrawingContext dc)
    {
        foreach (var shape in _fillSegments)
        {
            var points = shape.WorldPoints.Select(WorldToCanvas).ToList();
            if (points.Count > 2)
            {
                var geometry = new StreamGeometry();
                using (var ctx = geometry.Open())
                {
                    ctx.BeginFigure(points[0], true, true);
                    for (int i = 1; i < points.Count; i++)
                        ctx.LineTo(points[i], true, false);
                }
                geometry.Freeze();
                dc.DrawGeometry(shape.Fill, null, geometry);
            }
        }
    }
    
    private void DrawMidpointLine(DrawingContext dc, Point worldStart, Point worldEnd, Brush stroke, double thickness)
    {
        Point start = WorldToCanvas(worldStart);
        Point end = WorldToCanvas(worldEnd);

        // Convert to integer coordinates for Midpoint algorithm
        int x0 = (int)Math.Round(start.X);
        int y0 = (int)Math.Round(start.Y);
        int x1 = (int)Math.Round(end.X);
        int y1 = (int)Math.Round(end.Y);

        bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
        if (steep)
        {
            Swap(ref x0, ref y0);
            Swap(ref x1, ref y1);
        }
        if (x0 > x1)
        {
            Swap(ref x0, ref x1);
            Swap(ref y0, ref y1);
        }

        int dx = x1 - x0;
        int dy = Math.Abs(y1 - y0);
        int error = dx / 2;
        int ystep = (y0 < y1) ? 1 : -1;
        int y = y0;
        int radius = (int)Math.Ceiling(thickness / 2);

        for (int x = x0; x <= x1; x++)
        {
            if (steep)
            {
                DrawPoint(dc, y, x, stroke, radius);
            }
            else
            {
                DrawPoint(dc, x, y, stroke, radius);
            }
            error -= dy;
            if (error < 0)
            {
                y += ystep;
                error += dx;
            }
        }
    }

    private void DrawPoint(DrawingContext dc, int x, int y, Brush brush, int radius)
    {
        dc.DrawRectangle(brush, null, new Rect(
            x - radius, 
            y - radius, 
            radius * 2, 
            radius * 2
        ));
    }

    private Point WorldToCanvas(Point worldPoint)
    {
        double centerX = ActualWidth / 2;
        double centerY = ActualHeight / 2;
        
        return new Point(
            centerX + (worldPoint.X * Scale),
            centerY - (worldPoint.Y * Scale) // Invert Y for Cartesian
        );
    }

    private static void Swap<T>(ref T a, ref T b)
    {
        (a, b) = (b, a);
    }
    
    private void DrawAxes(DrawingContext dc)
    {
        var axisPen = new Pen(Brushes.Black, 1);
        double width = ActualWidth;
        double height = ActualHeight;

        // Draw X-axis
        dc.DrawLine(axisPen, new Point(0, Origin.Y), new Point(width, Origin.Y));

        // Draw Y-axis
        dc.DrawLine(axisPen, new Point(Origin.X, 0), new Point(Origin.X, height));

        // Draw ticks (every 5 pixels = 1 unit)
        double unit = Scale;
        for (double x = Origin.X; x < width; x += unit)
            dc.DrawLine(axisPen, new Point(x, Origin.Y - 3), new Point(x, Origin.Y + 3));

        for (double x = Origin.X; x > 0; x -= unit)
            dc.DrawLine(axisPen, new Point(x, Origin.Y - 3), new Point(x, Origin.Y + 3));

        for (double y = Origin.Y; y < height; y += unit)
            dc.DrawLine(axisPen, new Point(Origin.X - 3, y), new Point(Origin.X + 3, y));

        for (double y = Origin.Y; y > 0; y -= unit)
            dc.DrawLine(axisPen, new Point(Origin.X - 3, y), new Point(Origin.X + 3, y));
    }
}