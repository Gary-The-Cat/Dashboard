using SFML.System;
using Shared.DataStructures;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Shared.Helpers
{
    public static class MathsHelper
    {
        // Compute the dot product AB . BC
        private static double DotProduct(Vector2f pointA, Vector2f pointB, Vector2f pointC)
        {
            var AB = new Vector2f();
            var BC = new Vector2f();
            AB.X = pointB.X - pointA.X;
            AB.Y = pointB.Y - pointA.Y;
            BC.X = pointC.X - pointB.X;
            BC.Y = pointC.Y - pointB.Y;
            double dot = AB.X * BC.X + AB.Y * BC.Y;

            return dot;
        }

        // Compute the cross product AB x AC
        private static double CrossProduct(Vector2f pointA, Vector2f pointB, Vector2f pointC)
        {
            var AB = new Vector2f();
            var AC = new Vector2f();
            AB.X = pointB.X - pointA.X;
            AB.Y = pointB.Y - pointA.Y;
            AC.X = pointC.X - pointA.X;
            AC.Y = pointC.Y - pointA.Y;
            double cross = AB.X * AC.Y - AB.Y * AC.X;

            return cross;
        }

        //Compute the distance from A to B
        private static double Distance(Vector2f pointA, Vector2f pointB)
        {
            double d1 = pointA.X - pointB.X;
            double d2 = pointA.Y - pointB.Y;

            return Math.Sqrt(d1 * d1 + d2 * d2);
        }

        public static double LineToPointDistance2D(LineSegment line, Vector2f point)
        {
            return LineToPointDistance2D(line.Start, line.End, point);
        }
            
        //Compute the distance from AB to C
        //if isSegment is true, AB is a segment, not a line.
        public static double LineToPointDistance2D(Vector2f pointA, Vector2f pointB, Vector2f pointC, bool isSegment = true)
        {
            double dist = CrossProduct(pointA, pointB, pointC) / Distance(pointA, pointB);
            if (isSegment)
            {
                double dot1 = DotProduct(pointA, pointB, pointC);
                if (dot1 > 0)
                    return Distance(pointB, pointC);

                double dot2 = DotProduct(pointB, pointA, pointC);
                if (dot2 > 0)
                    return Distance(pointA, pointC);
            }
            return Math.Abs(dist);
        }

        // Find the point of intersection between the lines segmentA and segmentB.
        public static void FindIntersection(
            (Vector2f start, Vector2f end) segmentA, 
            (Vector2f start, Vector2f end) segmentB,
            out bool segments_intersect,
            out Vector2f intersection)
        {
            // Get the segments' parameters.
            float dx12 = segmentA.end.X - segmentA.start.X;
            float dy12 = segmentA.end.Y - segmentA.start.Y;
            float dx34 = segmentB.end.X - segmentB.start.X;
            float dy34 = segmentB.end.Y - segmentB.start.Y;

            // Solve for t1 and t2
            float denominator = (dy12 * dx34 - dx12 * dy34);

            float t1 =
                ((segmentA.start.X - segmentB.start.X) * dy34 + (segmentB.start.Y - segmentA.start.Y) * dx34)
                    / denominator;
            if (float.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                segments_intersect = false;
                intersection = new Vector2f(float.NaN, float.NaN);
                return;
            }

            float t2 =
                ((segmentB.start.X - segmentA.start.X) * dy12 + (segmentA.start.Y - segmentB.start.Y) * dx12)
                    / -denominator;

            // Find the point of intersection.
            intersection = new Vector2f(segmentA.start.X + dx12 * t1, segmentA.start.Y + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));
        }
    }
}
