using System;
using System.Threading.Tasks;
using System.Windows;
using WpfApp.TwoDimension;
using WpfApp.TwoDimension.Models;

namespace WpfApp.Animation
{
    public static class ShapeAnimation
    {
        private const int Duration = 500; // tổng thời gian animation
        private const int Steps = 30;     // số bước thực hiện animation

        public static async Task AnimateTranslate(ShapeContainer shape, Vector offset)
        {
            Vector step = new Vector(offset.X / Steps, offset.Y / Steps);

            for (int i = 0; i < Steps; i++)
            {
                shape.TransformShape(p => TransformationMatrix.Translate(p, step.X, step.Y));
                await Task.Delay(Duration / Steps);
            }
        }

        public static async Task AnimateScale(ShapeContainer shape, Point center, double scaleX, double scaleY)
        {
            double stepScaleX = Math.Pow(scaleX, 1.0 / Steps);
            double stepScaleY = Math.Pow(scaleY, 1.0 / Steps);

            for (int i = 0; i < Steps; i++)
            {
                shape.TransformShape(p =>
                {
                    // Dời p về gốc tọa độ dựa trên center
                    Vector v = p - center;

                    // Co giãn vector theo từng bước
                    Vector scaledV = new Vector(v.X * stepScaleX, v.Y * stepScaleY);

                    // Trả lại vị trí mới sau scale, cộng lại với center
                    return center + scaledV;
                });


                await Task.Delay(Duration / Steps);
            }
        }

        public static async Task AnimateRotate(ShapeContainer shape, Point center, double totalAngle)
        {
            double stepAngle = totalAngle / Steps;

            for (int i = 0; i < Steps; i++)
            {
                shape.TransformShape(p => TransformationMatrix.RotateAround(p, center, stepAngle));
                await Task.Delay(Duration / Steps);
            }
        }

        public static async Task AnimateSymmetric(ShapeContainer shape, Point symmetricPoint)
        {
            // Lấy tất cả các điểm ban đầu từ các đoạn
            List<Point> originalPoints = new();
            foreach (var seg in shape.Segments)
                originalPoints.AddRange(seg.WorldPoints);

            // Tính điểm sau đối xứng
            List<Point> targetPoints = new();
            foreach (var pt in originalPoints)
                targetPoints.Add(TransformationMatrix.Symmetric(pt, symmetricPoint));

            for (int step = 1; step <= Steps; step++)
            {
                double t = (double)step / Steps;

                int index = 0;
                shape.TransformShape(_ =>
                {
                    var start = originalPoints[index];
                    var end = targetPoints[index];
                    index++;
                    return new Point(
                        start.X + (end.X - start.X) * t,
                        start.Y + (end.Y - start.Y) * t
                    );
                });

                await Task.Delay(Duration / Steps);
            }
        }
    }
}
