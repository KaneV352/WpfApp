using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WpfApp.Utilities
{
    /// <summary>
    /// A custom Viewport3D control that displays a Cartesian axis system
    /// and includes built-in camera controls, with APIs for adding points and lines.
    /// </summary>
    public class Cartesian3dCanvas : Viewport3D
    {
        // The camera for the scene.
        private readonly PerspectiveCamera _camera;
        // The ModelVisual3D that holds the axis geometry.
        private readonly ModelVisual3D _axesModel;
        // ModelVisual3D that holds all user-added content
        private readonly ModelVisual3D _contentModel;
        
        // Fields for camera rotation
        private Point _lastMousePosition;
        private readonly Transform3DGroup _transformGroup;
        private readonly AxisAngleRotation3D _rotationX;
        private readonly AxisAngleRotation3D _rotationY;

        /// <summary>
        /// Initializes a new instance of the Cartesian3dCanvas class.
        /// </summary>
        public Cartesian3dCanvas()
        {
            // --- Initialize Camera ---
            _camera = new PerspectiveCamera
            {
                Position = new Point3D(12, 10, 20),
                LookDirection = new Vector3D(-8, -5, -15),
                UpDirection = new Vector3D(0, 1, 0),
                FieldOfView = 60
            };
            this.Camera = _camera;

            // --- Setup Camera Transforms for rotation ---
            _transformGroup = new Transform3DGroup();
            _rotationX = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            _rotationY = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);
            _transformGroup.Children.Add(new RotateTransform3D(_rotationY));
            _transformGroup.Children.Add(new RotateTransform3D(_rotationX));
            _camera.Transform = _transformGroup;

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

            // Generate the axes geometry
            CreateAxes();
        }

        /// <summary>
        /// Creates the 3D models for the X, Y, and Z axes and their ticks.
        /// </summary>
        private void CreateAxes()
        {
            var models = new Model3DGroup();
            double axisLength = 10.0;
            double tickSize = 0.2;
            int tickCount = 10;
            double lineThickness = 0.05;

            // X-Axis (Red)
            var xAxisMaterial = new DiffuseMaterial(Brushes.Red);
            models.Children.Add(CreateLine(new Point3D(0, 0, 0), new Point3D(axisLength, 0, 0), lineThickness, xAxisMaterial));
            for (int i = 1; i <= tickCount; i++)
            {
                models.Children.Add(CreateLine(new Point3D(i, -tickSize, 0), new Point3D(i, tickSize, 0), lineThickness, xAxisMaterial));
            }

            // Y-Axis (Green)
            var yAxisMaterial = new DiffuseMaterial(Brushes.Green);
            models.Children.Add(CreateLine(new Point3D(0, 0, 0), new Point3D(0, axisLength, 0), lineThickness, yAxisMaterial));
            for (int i = 1; i <= tickCount; i++)
            {
                models.Children.Add(CreateLine(new Point3D(-tickSize, i, 0), new Point3D(tickSize, i, 0), lineThickness, yAxisMaterial));
            }

            // Z-Axis (Blue)
            var zAxisMaterial = new DiffuseMaterial(Brushes.Blue);
            models.Children.Add(CreateLine(new Point3D(0, 0, 0), new Point3D(0, 0, axisLength), lineThickness, zAxisMaterial));
            for (int i = 1; i <= tickCount; i++)
            {
                models.Children.Add(CreateLine(new Point3D(0, -tickSize, i), new Point3D(0, tickSize, i), lineThickness, zAxisMaterial));
            }

            _axesModel.Content = models;
        }

        /// <summary>
        /// Creates a 3D cuboid to represent a line segment.
        /// </summary>
        private GeometryModel3D CreateLine(Point3D start, Point3D end, double thickness, Material material)
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
                0, 1, 2, 0, 2, 3, 4, 7, 6, 4, 6, 5,
                0, 4, 5, 0, 5, 1, 1, 5, 6, 1, 6, 2,
                2, 6, 7, 2, 7, 3, 3, 7, 4, 3, 4, 0
            };
            foreach (var index in indices) mesh.TriangleIndices.Add(index);

            return new GeometryModel3D(mesh, material);
        }

        #region Shape Management APIs
        
        /// <summary>
        /// Adds a point to the 3D scene
        /// </summary>
        /// <param name="position">Position of the point in 3D space</param>
        /// <param name="color">Color of the point</param>
        /// <param name="size">Size (diameter) of the point</param>
        public void AddPoint(Point3D position, Color color, double size = 0.2)
        {
            var sphere = CreateSphere(position, size / 2, new SolidColorBrush(color));
            AddToContent(sphere);
        }

        /// <summary>
        /// Adds a line to the 3D scene
        /// </summary>
        /// <param name="start">Start point of the line</param>
        /// <param name="end">End point of the line</param>
        /// <param name="color">Color of the line</param>
        /// <param name="thickness">Thickness of the line</param>
        public void AddLine(Point3D start, Point3D end, Color color, double thickness = 0.05)
        {
            var line = CreateLine(start, end, thickness, new DiffuseMaterial(new SolidColorBrush(color)));
            AddToContent(line);
        }

        /// <summary>
        /// Creates a sphere geometry
        /// </summary>
        private GeometryModel3D CreateSphere(Point3D center, double radius, Brush brush)
        {
            var mesh = new MeshGeometry3D();
            int stacks = 12, slices = 12;

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
                    mesh.Normals.Add(new Vector3D(x, y, z));
                }
            }

            // Generate sphere triangles
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

            return new GeometryModel3D(mesh, new DiffuseMaterial(brush));
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

        /// <summary>
        /// Clears all user-added content from the scene
        /// </summary>
        public void ClearContent()
        {
            if (_contentModel.Content is Model3DGroup group)
            {
                group.Children.Clear();
            }
        }

        #endregion

        #region Camera Controls

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.CaptureMouse();
            _lastMousePosition = e.GetPosition(this);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            this.ReleaseMouseCapture();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.IsMouseCaptured)
            {
                Point currentPosition = e.GetPosition(this);
                double dx = currentPosition.X - _lastMousePosition.X;
                double dy = currentPosition.Y - _lastMousePosition.Y;
                double sensitivity = 0.5;

                _rotationX.Angle += dx * sensitivity;
                _rotationY.Angle += dy * sensitivity;

                _lastMousePosition = currentPosition;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            double zoomFactor = e.Delta > 0 ? 0.9 : 1.1;
            Point3D currentPos = _camera.Position;
            Vector3D lookDir = _camera.LookDirection;
            lookDir.Normalize();

            // Move camera along its look direction
            _camera.Position = new Point3D(
                currentPos.X - (1 - zoomFactor) * lookDir.X * 10,
                currentPos.Y - (1 - zoomFactor) * lookDir.Y * 10,
                currentPos.Z - (1 - zoomFactor) * lookDir.Z * 10
            );
        }
        #endregion
    }
}