using System.Windows.Media.Media3D;

namespace WpfApp.ThreeDimension;

public static class TransformationMatrix3D
{
    /// <summary>
    /// Translates a 3D point by a given 3D vector.
    /// </summary>
    /// <param name="point">The original <see cref="Point3D"/> to translate.</param>
    /// <param name="translation">The <see cref="Vector3D"/> representing the translation offset.</param>
    /// <returns>
    /// A new <see cref="Point3D"/> that is the result of translating the original point by the specified vector.
    /// </returns>
    public static Point3D Translate(Point3D point, Vector3D translation)
    {
        var matrix = new Matrix3D(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            translation.X, translation.Y, translation.Z, 1);
        return matrix.Transform(point);
    }
    
    /// <summary>
    /// Rotates a 3D point around a specified axis by a given angle.
    /// </summary>
    /// <param name="point">The <see cref="Point3D"/> to rotate.</param>
    /// <param name="axis">The <see cref="Vector3D"/> representing the axis of rotation.</param>
    /// <param name="angle">The angle of rotation in degrees.</param>
    /// <returns>
    /// A new <see cref="Point3D"/> that is the result of rotating the original point around the specified axis.
    /// </returns>
    public static Point3D RotateAroundAxis(Point3D point, Vector3D axis, double angle)
    {
        var rotationMatrix = new Matrix3D();
        rotationMatrix.Rotate(new Quaternion(axis, angle));
        return rotationMatrix.Transform(point);
    }
    
    /// <summary>
    /// Scales a 3D point by the specified scale factors along each axis.
    /// </summary>
    /// <param name="point">The <see cref="Point3D"/> to scale.</param>
    /// <param name="scaleX">The scale factor along the X-axis.</param>
    /// <param name="scaleY">The scale factor along the Y-axis.</param>
    /// <param name="scaleZ">The scale factor along the Z-axis.</param>
    /// <returns>
    /// A new <see cref="Point3D"/> that is the result of scaling the original point.
    /// </returns>
    public static Point3D Scale(Point3D point, double scaleX, double scaleY, double scaleZ)
    {
        var scaleMatrix = new Matrix3D(
            scaleX, 0, 0, 0,
            0, scaleY, 0, 0,
            0, 0, scaleZ, 0,
            0, 0, 0, 1);
        return scaleMatrix.Transform(point);
    }
    
    /// <summary>
    /// Calculates the symmetric point of a given 3D point with respect to another point.
    /// </summary>
    /// <param name="point">The <see cref="Point3D"/> to find the symmetric for.</param>
    /// <param name="symmetricPoint">The <see cref="Point3D"/> to use as the center of symmetry.</param>
    /// <returns>
    /// A new <see cref="Point3D"/> that is symmetric to the original point with respect to the specified center.
    /// </returns>
    public static Point3D Symmetric(Point3D point, Point3D symmetricPoint)
    {
        var translationToOrigin = new Vector3D(-symmetricPoint.X, -symmetricPoint.Y, -symmetricPoint.Z);
        var translationBack = new Vector3D(symmetricPoint.X, symmetricPoint.Y, symmetricPoint.Z);
        
        // Translate to origin, reflect, then translate back
        var matrix = new Matrix3D(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            translationToOrigin.X, translationToOrigin.Y, translationToOrigin.Z, 1);
        
        var reflectedPoint = matrix.Transform(point);
        
        matrix.OffsetX = translationBack.X;
        matrix.OffsetY = translationBack.Y;
        matrix.OffsetZ = translationBack.Z;
        
        return matrix.Transform(reflectedPoint);
    }
}