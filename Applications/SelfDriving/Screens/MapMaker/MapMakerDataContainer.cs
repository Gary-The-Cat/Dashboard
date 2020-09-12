using SelfDriving.Shared;
using SFML.Graphics;
using SFML.System;
using Shared.ExtensionMethods;
using Shared.Helpers;
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

        public IEnumerable<(Guid segmentId, Vector2f start, Vector2f end)> segments =>
            trackSegments.Select(v => (v.Key, v.Value[0].Position, v.Value[1].Position));

        public MapMakerDataContainer()
        {
            trackSegments = new Dictionary<Guid, Vertex[]>();
        }

        public Guid AddSegment(Vector2f startPoint, Vector2f endPoint, bool isTrack)
        {
            var segment = new Vertex[2];
            segment[0] = new Vertex() { Color = isTrack ? Color.Black : Color.Blue, Position = startPoint };
            segment[1] = new Vertex() { Color = isTrack ? Color.Black : Color.Blue, Position = endPoint };
            var segmentId = Guid.NewGuid();
            trackSegments.Add(segmentId, segment);

            return segmentId;
        }

        public bool RemoveSegment(Guid segmentId)
        {
            return trackSegments.Remove(segmentId);
        }

        public void SetSegmentEnd(Guid segmentId, Vector2f endPoint)
        {
            SetVertexPosition(segmentId, 1, endPoint);
        }

        public void SetVertexPosition(Guid segmentId, int vertexId, Vector2f position)
        {
            var segment = trackSegments[segmentId];
            segment[vertexId].Position = position;
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

        public (Guid?, double) GetNearestLine(Vector2f point, bool isDrawing)
        {
            Guid? nearestPoint = null;
            var closestDistance = float.MaxValue;
            var lastSegment = segments.Last();

            foreach (var segment in segments)
            {
                if (isDrawing && segment == lastSegment)
                {
                    continue;
                }

                var distance = (float)MathsHelper.LineToPointDistance2D(segment.start, segment.end, point);

                if (distance < closestDistance)
                {
                    nearestPoint = segment.segmentId;
                    closestDistance = distance;
                }
            }

            return (nearestPoint, closestDistance);
        }

        public List<(Guid, int)> GetSegmentsContaining(Vector2f point)
        {
            var segmentsContainingPoint = new List<(Guid, int)>();

            foreach (var segment in trackSegments)
            {
                if (segment.Value[0].Position == point)
                {
                    segmentsContainingPoint.Add((segment.Key, 0));
                }

                if (segment.Value[1].Position == point)
                {
                    segmentsContainingPoint.Add((segment.Key, 1));
                }
            }

            return segmentsContainingPoint;
        }

        public (Vector2f start, Vector2f end) TrimSegment(Guid currentSegmentId)
        {
            var (start, end) = GetSegment(currentSegmentId);
            bool startSet = false;
            bool endSet = false;

            foreach (var segment in segments)
            {
                if (segment.segmentId == currentSegmentId)
                {
                    continue;
                }

                MathsHelper.FindIntersection(
                    (start, end),
                    (segment.start, segment.end),
                    out var intersects,
                    out var intersectPoint);

                if (intersects)
                {
                    if (!startSet)
                    {
                        startSet = true;
                        start = intersectPoint;
                    }
                    else
                    {
                        end = intersectPoint;
                        endSet = true;
                        break;
                    }
                }
            }

            if (startSet && endSet)
            {
                // Update the visual for the current segement.
                var currentSegment = trackSegments[currentSegmentId];
                currentSegment[0].Position = start;
                currentSegment[1].Position = end;

                return (start, end);
            }
            else
            {
                return GetSegment(currentSegmentId);
            }
        }
    }
}
