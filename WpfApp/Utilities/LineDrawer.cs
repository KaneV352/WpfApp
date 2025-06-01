using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp.Utilities;

public static class LineDrawer
{
    /// <summary>
    /// Draws a line on the canvas from (x1, y1) to (x2, y2)
    /// </summary>
    /// <param name="canvas">The canvas to draw on</param>
    /// <param name="point1">The starting point</param>
    /// <param name="point2">The end point</param>
    /// <param name="color">Color of the line</param>
    /// <param name="stepIncrement"></param>
    /// <param name="thickness">Thickness of the line</param>
    public static void DrawLine(
        Canvas canvas,
        Point point1,
        Point point2,
        Brush color,
        double stepIncrement,
        double thickness = 1.0
    )
    {
        if (stepIncrement <= 0.00001)
            stepIncrement = 1.0; // Safety

        double currentX = point1.X;
        double currentY = point1.Y;

        // These are properties of the whole line, calculate once
        int dxTotal = (int)Math.Abs(point2.X - point1.X);
        int sxDir = (point1.X < point2.X) ? 1 : -1;
        int dyTotal = (int)Math.Abs(point2.Y - point1.Y);
        int syDir = (point1.Y < point2.Y) ? 1 : -1;
        int negAbsDyTotal = -dyTotal;
        int err = dxTotal + negAbsDyTotal; // Initial error

        var pointToDraw = new Point(currentX, currentY); // Re-use this Point object
        PointDrawer.DrawPoint(canvas, pointToDraw, color, size: thickness);

        int safetyBreakCounter = 0; // Add a safety break for iterations
        int maxIterations = (dxTotal + dyTotal + 2) * 2; // Heuristic, adjust if stepIncrement is very small
        if (stepIncrement > 0.001 && (dxTotal > 0 || dyTotal > 0))
        {
            maxIterations = (int)(Math.Max(dxTotal, dyTotal) / stepIncrement) + 5; // Steps along dominant axis + buffer
            if (maxIterations < 10)
                maxIterations = 10000; // Ensure a reasonable minimum if deltas are small
        }
        else if (dxTotal == 0 && dyTotal == 0)
        {
            maxIterations = 1;
        }

        while (Math.Abs(currentX - point2.X) > 0.001 || Math.Abs(currentY - point2.Y) > 0.001)
        {
            safetyBreakCounter++;
            if (safetyBreakCounter > maxIterations && maxIterations > 1)
            { // Avoid break if point1=point2
                // Log error: "Max iterations reached"
                break;
            }

            double prevCurrentX = currentX; // For stall detection
            double prevCurrentY = currentY;

            if ((sxDir * (point2.X - currentX)) < 0.001 && (syDir * (point2.Y - currentY)) < 0.001)
            {
                break;
            }

            int e2 = 2 * err;

            // --- This block determines which direction Bresenham err term says to go ---
            // --- And updates err. It does NOT yet update currentX/Y ---
            bool xStepDecided = false;
            bool yStepDecided = false;

            if (e2 >= negAbsDyTotal)
            {
                if (sxDir * (point2.X - currentX) > 0.001 || dxTotal == 0)
                { // Can X move towards target?
                    err += negAbsDyTotal;
                    xStepDecided = true;
                }
            }

            if (e2 <= dxTotal)
            {
                if (syDir * (point2.Y - currentY) > 0.001 || dyTotal == 0)
                { // Can Y move towards target?
                    err += dxTotal;
                    yStepDecided = true;
                }
            }

            // --- Now calculate actual new positions based on decided steps ---
            double tentativeX = currentX;
            double tentativeY = currentY;

            if (xStepDecided)
            {
                tentativeX = currentX + sxDir * stepIncrement;
            }
            if (yStepDecided)
            {
                // If X also stepped, this Y increment is from the original currentY for this iteration's decision phase
                tentativeY = currentY + syDir * stepIncrement;
            }

            // Clamp
            if (sxDir > 0)
                currentX = Math.Min(tentativeX, point2.X);
            else
                currentX = Math.Max(tentativeX, point2.X);

            if (syDir > 0)
                currentY = Math.Min(tentativeY, point2.Y);
            else
                currentY = Math.Max(tentativeY, point2.Y);

            // Stall detection
            if (
                Math.Abs(currentX - prevCurrentX) < 0.00001
                && Math.Abs(currentY - prevCurrentY) < 0.00001
            )
            {
                break; // Stall
            }

            pointToDraw.X = currentX;
            pointToDraw.Y = currentY;
            PointDrawer.DrawPoint(canvas, pointToDraw, color, size: thickness);
        }

        // Final endpoint draw
        if (
            Math.Abs(currentX - point2.X) > 0.001
            || Math.Abs(currentY - point2.Y) > 0.001
            || (
                (int)Math.Round(currentX) != (int)Math.Round(point2.X)
                || (int)Math.Round(currentY) != (int)Math.Round(point2.Y)
            )
        )
        {
            pointToDraw.X = point2.X;
            pointToDraw.Y = point2.Y;
            PointDrawer.DrawPoint(canvas, pointToDraw, color, size: thickness);
        }
    }
}
