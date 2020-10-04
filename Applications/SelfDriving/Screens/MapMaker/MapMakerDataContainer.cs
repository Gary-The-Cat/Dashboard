using SelfDriving.Shared;
using SFML.Graphics;
using SFML.System;
using Shared.DataStructures;
using Shared.ExtensionMethods;
using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public Vector2f StartPosition { get; set; }

        public float StartRotation { get; set; }

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

        public List<LineSegment> GetMap()
        {
            var segments = trackSegments.Where(s => s.Value[0].Color == Color.Black).
                Select(s => new LineSegment(s.Value[0].Position, s.Value[1].Position));

            return segments.ToList();
        }

        public List<LineSegment> GetCheckpoints()
        {
            var segments = trackSegments.Where(s => s.Value[0].Color == Color.Blue).
                Select(s => new LineSegment(s.Value[0].Position, s.Value[1].Position));

            return segments.ToList();
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

        public double GetSegmentLength(Guid segmentId)
        {
            var (start, end) = GetSegment(segmentId);

            return start.Distance(end);
        }

        public double GetSegmentAngle(Guid segmentId)
        {
            var (start, end) = GetSegment(segmentId);

            return (end - start).GetAngle();
        }

        public (Vector2f?, double) GetNearestPoint(Vector2f position, bool isDrawing)
        {
            Vector2f? nearestPoint = null;
            var closestDistance = float.MaxValue;

            if (!vertexPositions.Any())
            {
                return (nearestPoint, closestDistance);
            }

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

        public void Clear()
        {
            trackSegments.Clear();
            StartPosition = new Vector2f();
            EditState = MapEditState.DrawingLines;
        }

        public (Guid?, double) GetNearestLine(Vector2f point, bool isDrawing)
        {
            Guid? nearestPoint = null;
            var closestDistance = float.MaxValue;

            if (!segments.Any())
            {
                return (nearestPoint, closestDistance);
            }

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
            var intersectionPoints = new List<Vector2f>();

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
                    intersectionPoints.Add(intersectPoint);
                }
            }

            if (intersectionPoints.Count() == 2)
            {
                // Update the visual for the current segement.
                var currentSegment = trackSegments[currentSegmentId];
                var trimmedStart = intersectionPoints[0];
                var trimmedEnd = intersectionPoints[1];
                currentSegment[0].Position = trimmedStart;
                currentSegment[1].Position = trimmedEnd;

                return (trimmedStart, trimmedEnd);
            }
            else
            {
                return GetSegment(currentSegmentId);
            }
        }
    }
}
