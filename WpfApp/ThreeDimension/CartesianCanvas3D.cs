using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using WpfApp.ThreeDimension.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel; // Required for INotifyPropertyChanged

namespace WpfApp.ThreeDimension;

/// <summary>
/// A custom Viewport3D control that displays a Cartesian axis system
/// and includes built-in camera controls, with APIs for adding points and lines.
/// </summary>
public class CartesianCanvas3D : Viewport3D
{
    // The ModelVisual3D that holds the axis geometry.
    private readonly ModelVisual3D _axesModel;
    // ModelVisual3D that holds all user-added content
    private readonly ModelVisual3D _contentModel;

    private List<PointSegment3D> _points = new();
    private List<LineSegment3D> _lines = new();

    private MatrixTransform3D _shearTransform;
    public bool IsObliqueProjection { get; private set; }

    // Constants for rendering geometry (made private fields for consistency)
    private const int SphereDivisions = 8; // How detailed the sphere is (e.g., 8 for a low-poly sphere)
    private const int LineUnitPerDistance = 20; // How many points to draw per unit distance for lines

    // Axis parameters (made private fields for accessibility across methods)
    private double _axisLength = 40.0;
    private double _axisLengthZ = 80.0; // Length of Z-axis
    private double _tickSize = 0.2;
    private double _lineThickness = 0.05;


    /// <summary>
    /// Initializes a new instance of the Cartesian3dCanvas class.
    /// </summary>
    public CartesianCanvas3D()
    {
        // --- Setup Lights ---
        var lights = new ModelVisual3D
        {
            Content = new Model3DGroup()
        };
        ((Model3DGroup)lights.Content).Children.Add(new AmbientLight(Colors.White));
        this.Children.Add(lights);

        // --- Setup Axes Model ---
        _axesModel = new ModelVisual3D();
        this.Children.Add(_axesModel);

        // --- Setup Content Model ---
        _contentModel = new ModelVisual3D
        {
            Content = new Model3DGroup()
        };
        this.Children.Add(_contentModel);
        
        SetCabinetView(); // Set initial view to Cabinet projection
    }

    #region Shape Management APIs

    public PointSegment3D AddPoint(Point3D position, Color color, double size = 0.1)
    {
        DrawPoint(position, color, size);

        var segment = new PointSegment3D(position, color, size);
        _points.Add(segment);
        segment.PropertyChanged += OnSegmentPointsChanged;
        return segment;
    }

    private void DrawPoint(Point3D position, Color color, double size)
    {
        var point = CreateSphere(position, size / 2, new SolidColorBrush(color));
        AddToContent(point);
    }

    public LineSegment3D AddLine(Point3D p1, Point3D p2, Color color, double thickness = 0.2)
    {
        double distance = (p2 - p1).Length;
        int numPoints = (int)(distance * LineUnitPerDistance);
        DrawLine(p1, p2, color, thickness, numPoints);

        var segment = new LineSegment3D(p1, p2, color, numPoints, thickness / 2);
        _lines.Add(segment);
        segment.PropertyChanged += OnSegmentPointsChanged;
        return segment;
    }

    private void DrawLine(Point3D p1, Point3D p2, Color color, double thickness, int numPoints)
    {
        if (numPoints < 2) numPoints = 2; // Ensure at least the two endpoints are drawn.

        for (int i = 0; i < numPoints; i++)
        {
            double t = (double)i / (numPoints - 1); // t ranges from 0 to 1, including both p1 and p2
            Point3D interpolatedPoint = new Point3D(
                p1.X + t * (p2.X - p1.X),
                p1.Y + t * (p2.Y - p1.Y),
                p1.Z + t * (p2.Z - p1.Z)
            );
            var point = CreateSphere(interpolatedPoint, thickness / 2, new SolidColorBrush(color)); // Call DrawPoint for each interpolated point
            AddToContent(point);
        }
    }

    /// <summary>
    /// Creates a sphere geometry
    /// </summary>
    private GeometryModel3D CreateSphere(Point3D center, double radius, Brush brush)
    {
        var mesh = CreateSphereGeometry(center, radius);

        var material = new DiffuseMaterial(brush);
        return new GeometryModel3D(mesh, material);
    }

    /// <summary>
    /// Clears all user-added content from the scene
    /// </summary>
    public void ClearContent()
    {
        foreach (var point in _points)
        {
            point.PropertyChanged -= OnSegmentPointsChanged;
        }
        foreach (var line in _lines)
        {
            line.PropertyChanged -= OnSegmentPointsChanged;
        }

        _points.Clear();
        _lines.Clear();
        if (_contentModel.Content is Model3DGroup group)
        {
            group.Children.Clear();
        }
    }

    private MeshGeometry3D CreateSphereGeometry(Point3D center, double radius)
    {
        var mesh = new MeshGeometry3D();
        int stacks = SphereDivisions, slices = SphereDivisions;

        // Generate sphere vertices
        for (int stack = 0; stack <= stacks; stack++)
        {
            double phi = Math.PI / 2 - stack * Math.PI / stacks;
            double y = radius * Math.Sin(phi);
            double scale = -radius * Math.Cos(phi);

            for (int slice = 0; slice <= slices; slice++)
            {
                double theta = slice * 2 * Math.PI / slices;
                double x = scale * Math.Sin(theta);
                double z = scale * Math.Cos(theta);

                mesh.Positions.Add(new Point3D(center.X + x, center.Y + y, center.Z + z));
                mesh.Normals.Add(new Vector3D(x, y, z)); // Normal vectors for lighting
            }
        }

        // Generate sphere triangles (faces)
        for (int stack = 0; stack < stacks; stack++)
        {
            for (int slice = 0; slice < slices; slice++)
            {
                int first = (stack * (slices + 1)) + slice;
                int second = first + slices + 1;

                mesh.TriangleIndices.Add(first);
                mesh.TriangleIndices.Add(second);
                mesh.TriangleIndices.Add(first + 1);

                mesh.TriangleIndices.Add(second);
                mesh.TriangleIndices.Add(second + 1);
                mesh.TriangleIndices.Add(first + 1);
            }
        }

        return mesh;
    }

    /// <summary>
    /// Adds a 3D model to the content visual
    /// </summary>
    private void AddToContent(GeometryModel3D model)
    {
        if (_contentModel.Content is Model3DGroup group)
        {
            group.Children.Add(model);
        }
    }

    private void OnSegmentPointsChanged(object? sender, EventArgs e)
    {
        Console.WriteLine("Segment points changed, updating content model.");
        if (_contentModel.Content is Model3DGroup group)
        {
            group.Children.Clear();
            foreach (var point in _points)
            {
                DrawPoint(point.Position, point.Color, point.Size);
            }
            foreach (var line in _lines)
            {
                DrawLine(line.StartPoint, line.EndPoint, line.Color, line.Thickness * 2, line.NumSegments);
            }
        }
    }

    #endregion

    #region Camera Controls

    public void SetCavalierView()
    {
        ResetView();
        ApplyObliqueProjection(shearX: -0.707, shearY: -0.707); // cos(45°)=sin(45°)=0.707
    }

    public void SetCabinetView()
    {
        ResetView();
        ApplyObliqueProjection(shearX: -0.354, shearY: -0.354); // 0.5 * 0.707
    }

    public void SetFrontView()
    {
        ResetView();;
        // Remove Z axis 
        if (_axesModel.Content is Model3DGroup axesGroup)
        {
            axesGroup.Children.RemoveAt(2); // Assuming Z-axis is the third child
        }
            
        var frontCamera = new OrthographicCamera()
        {
            Position = new Point3D(0.5, 0.5, 10),
            LookDirection = new Vector3D(0, 0, -10),
            UpDirection = new Vector3D(0, 1, 0),
            Width = 20,
        };
        Camera = frontCamera;
    }

    public void SetTopView()
    {
        ResetView();
        // Remove Y axis 
        if (_axesModel.Content is Model3DGroup axesGroup)
        {
            axesGroup.Children.RemoveAt(1); // Assuming Y-axis is the second child
        }
            
        var topCamera = new OrthographicCamera()
        {
            Position = new Point3D(0.5, 10, 0.5),
            LookDirection = new Vector3D(0, -10, 0),
            UpDirection = new Vector3D(0, 0, 1),
            Width = 20,
        };
        Camera = topCamera;
    }

    public void SetSideView()
    {
        ResetView();
        // Remove X axis 
        if (_axesModel.Content is Model3DGroup axesGroup)
        {
            axesGroup.Children.RemoveAt(0); // Assuming X-axis is the first child
        }
            
        var sideCamera = new OrthographicCamera()
        {
            Position = new Point3D(10, 0.5, 0.5),
            LookDirection = new Vector3D(-10, 0, 0),
            UpDirection = new Vector3D(0, 1, 0),
            Width = 20,
        };
        Camera = sideCamera;
    }

    #endregion

    #region Private Methods

    private void ApplyObliqueProjection(double shearX, double shearY)
    {
        IsObliqueProjection = true;

        // Create shear matrix
        Matrix3D shearMatrix = new Matrix3D(
            1, 0, 0, 0,
            0, 1, 0, 0,
            shearX, shearY, 1, 0, // This is where the shear happens on Z
            0, 0, 0, 1
        );

        _shearTransform = new MatrixTransform3D(shearMatrix);
        _axesModel.Transform = _shearTransform;
        _contentModel.Transform = _shearTransform;

        // --- Set Orthographic Camera for Oblique Projection ---
        // Calculate the effective X and Y span of the scene after applying the shear to the Z-axis.
        // This ensures the camera's view frustum can encompass the entire transformed scene.
        double minX = Math.Min(0, shearX * _axisLengthZ);
        double maxX = Math.Max(_axisLength, shearX * _axisLengthZ);
        double minY = Math.Min(0, shearY * _axisLengthZ);
        double maxY = Math.Max(_axisLength, shearY * _axisLengthZ);

        double spanX = maxX - minX;
        double spanY = maxY - minY;

        // Determine the camera's width based on the largest span, with some padding.
        double effectiveCameraWidth = Math.Max(spanX, spanY) * 1.2;

        Camera = new OrthographicCamera()
        {
            // Position the camera to center the view around the transformed scene.
            // The Z-component is set far enough back to ensure the entire Z-axis is visible.
            Position = new Point3D(
                (minX + maxX) / 2, // Center X of the view
                (minY + maxY) / 2, // Center Y of the view
                _axisLengthZ + 100 // Position far back along Z to ensure positive Z-axis is seen
            ),
            LookDirection = new Vector3D(0, 0, -1), // Look towards the origin (negative Z direction)
            UpDirection = new Vector3D(0, 1, 0),
            Width = effectiveCameraWidth, // Use the calculated effective width
        };
    }

    private void ResetView()
    {
        _axesModel.Transform = Transform3D.Identity;
        _contentModel.Transform = Transform3D.Identity;
        IsObliqueProjection = false;

        if (_axesModel.Content is Model3DGroup group)
            group.Children.Clear();

        CreateAxes();
    }

    /// <summary>
    /// Creates the 3D models for the X, Y, and Z axes and their ticks.
    /// </summary>
    private void CreateAxes()
    {
        var models = new Model3DGroup();

        // X-Axis (Red)
        var xAxisMaterial = new DiffuseMaterial(Brushes.Red);
        models.Children.Add(DrawLine(new Point3D(0, 0, 0), new Point3D(_axisLength, 0, 0), _lineThickness, xAxisMaterial));
        for (int i = 1; i <= (int)_axisLength; i++)
        {
            models.Children.Add(DrawLine(new Point3D(i, -_tickSize, 0), new Point3D(i, _tickSize, 0), _lineThickness, xAxisMaterial));
        }

        // Y-Axis (Green)
        var yAxisMaterial = new DiffuseMaterial(Brushes.Green);
        models.Children.Add(DrawLine(new Point3D(0, 0, 0), new Point3D(0, _axisLength, 0), _lineThickness, yAxisMaterial));
        for (int i = 1; i <= (int)_axisLength; i++)
        {
            models.Children.Add(DrawLine(new Point3D(-_tickSize, i, 0), new Point3D(_tickSize, i, 0), _lineThickness, yAxisMaterial));
        }

        // Z-Axis (Blue)
        var zAxisMaterial = new DiffuseMaterial(Brushes.Blue);
        models.Children.Add(DrawLine(new Point3D(0, 0, 0), new Point3D(0, 0, _axisLengthZ), _lineThickness, zAxisMaterial));
        for (int i = 1; i <= (int)_axisLengthZ; i++) // Use _axisLengthZ for Z-axis tick count
        {
            models.Children.Add(DrawLine(new Point3D(0, -_tickSize, i), new Point3D(0, _tickSize, i), _lineThickness, zAxisMaterial));
        }

        _axesModel.Content = models;
    }

    /// <summary>
    /// Creates a 3D cuboid to represent a line segment.
    /// </summary>
    private GeometryModel3D DrawLine(Point3D start, Point3D end, double thickness, Material material)
    {
        var mesh = new MeshGeometry3D();

        Vector3D direction = end - start;
        Vector3D up = Math.Abs(direction.X) > 0.01 || Math.Abs(direction.Z) > 0.01 ? new Vector3D(0, 1, 0) : new Vector3D(1, 0, 0);

        Vector3D side = Vector3D.CrossProduct(direction, up);
        side.Normalize();
        side *= thickness / 2;

        up = Vector3D.CrossProduct(direction, side);
        up.Normalize();
        up *= thickness / 2;

        Point3D[] points = {
            start - side - up, start + side - up, start + side + up, start - side + up,
            end - side - up, end + side - up, end + side + up, end - side + up
        };
        foreach (var p in points) mesh.Positions.Add(p);

        int[] indices = {
            0, 1, 2, 0, 2, 3, // Front face
            4, 7, 6, 4, 6, 5, // Back face
            0, 4, 5, 0, 5, 1, // Bottom face
            1, 5, 6, 1, 6, 2, // Right face
            2, 6, 7, 2, 7, 3, // Top face
            3, 7, 4, 3, 4, 0  // Left face
        };
        foreach (var index in indices) mesh.TriangleIndices.Add(index);

        return new GeometryModel3D(mesh, material);
    }
    #endregion
}