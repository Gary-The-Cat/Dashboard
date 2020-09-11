using SFML.System;
using System;

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
    }
}
