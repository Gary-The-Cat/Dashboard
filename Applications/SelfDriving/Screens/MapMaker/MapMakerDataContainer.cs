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

        public Dictionary<Guid, Vertex[]> trackSegments { get; set; }

        public IEnumerable<(Vector2f start, Vector2f end)> vertexPositions => 
            trackSegments.Values.Select(v => (v[0].Position, v[1].Position));

        public MapMakerDataContainer()
        {
            trackSegments = new Dictionary<Guid, Vertex[]>();
        }

        public Guid AddTrackSegment(Vector2f startPoint, Vector2f endPoint)
        {
            var segment = new Vertex[2];
            segment[0] = new Vertex() { Color = Color.Black, Position = startPoint };
            segment[1] = new Vertex() { Color = Color.Black, Position = endPoint };
            var segmentId = Guid.NewGuid();
            trackSegments.Add(segmentId, segment);

            return segmentId;
        }

        public void SetSegmentEnd(Guid segmentId, Vector2f endPoint)
        {
            var segment = trackSegments[segmentId];
            segment[1].Position = endPoint;
        }

        public (Vector2f start, Vector2f end) GetSegment(Guid segmentId)
        {
            var segment = trackSegments[segmentId];
            var startPos = segment[0].Position;
            var endPos = segment[1].Position;

            return (startPos, endPos);
        }

        public double GetCurrentSegmentLength()
        {
            var (start, end) = vertexPositions.Last();

            return start.Distance(end);
        }

        public double GetCurrentSegmentAngle()
        {
            var (start, end) = vertexPositions.Last();

            return (end - start).GetAngle();
        }

        public (Vector2f?, double) GetNearestPoint(Vector2f position, bool isDrawing)
        {
            Vector2f? nearestPoint = null;
            var closestDistance = float.MaxValue;
            var lastSegment = vertexPositions.Last();

            foreach (var segment in vertexPositions)
            {
                if (isDrawing && segment == lastSegment)
                {
                    continue;
                }

                var startPos = segment.start;
                var distanceToStart = position.Distance(startPos);

                if (distanceToStart < closestDistance && startPos != position)
                {
                    nearestPoint = startPos;
                    closestDistance = distanceToStart;
                }

                var endPos = segment.end;
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
