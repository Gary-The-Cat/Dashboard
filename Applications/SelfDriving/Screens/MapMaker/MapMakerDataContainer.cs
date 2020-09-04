using SelfDriving.Shared;
using SFML.Graphics;
using SFML.System;
using Shared.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelfDriving.Screens.MapMaker
{
    public class MapMakerDataContainer
    {
        public MapEditState EditState { get; set; }

        public List<Vertex[]> trackSegments { get; set; }

        public MapMakerDataContainer()
        {
            trackSegments = new List<Vertex[]>();
        }

        public void AddTrackSegment(Vector2f startPoint, Vector2f endPoint)
        {
            var segment = new Vertex[2];
            segment[0] = new Vertex() { Color = Color.Black, Position = startPoint };
            segment[1] = new Vertex() { Color = Color.Black, Position = endPoint };
            trackSegments.Add(segment);
        }

        public void SetCurrentSegmentEnd(Vector2f endPoint)
        {
            var segment = trackSegments.Last();
            segment[1].Position = endPoint;
        }

        public (Vector2f start, Vector2f end) GetCurrentSegment()
        {
            var segment = trackSegments.Last();
            var startPos = segment[0].Position;
            var endPos = segment[1].Position;

            return (startPos, endPos);
        }

        public double GetCurrentSegmentLength()
        {
            var (start, end) = GetCurrentSegment();

            return start.Distance(end);
        }

        public double GetCurrentSegmentAngle()
        {
            var (start, end) = GetCurrentSegment();

            return (end - start).GetAngle();
        }

        public (Vector2f?, double) GetNearestPoint(Vector2f position, bool isDrawing)
        {
            Vector2f? nearestPoint = null;
            var closestDistance = float.MaxValue;

            foreach (var segment in trackSegments)
            {
                if (isDrawing && segment == trackSegments.Last())
                {
                    continue;
                }

                var startPos = segment[0].Position;
                var distanceToStart = position.Distance(startPos);

                if (distanceToStart < closestDistance && startPos != position)
                {
                    nearestPoint = startPos;
                    closestDistance = distanceToStart;
                }

                var endPos = segment[1].Position;
                var distanceToEnd = position.Distance(endPos);

                if (distanceToEnd < closestDistance && endPos != position)
                {
                    nearestPoint = endPos;
                    closestDistance = distanceToEnd;
                }

            }

            return (nearestPoint, closestDistance);
        }
    }
}
