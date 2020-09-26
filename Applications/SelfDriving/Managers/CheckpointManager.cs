using SelfDriving.Shared;
using SFML.System;
using Shared.DataStructures;
using Shared.ExtensionMethods;
using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SelfDriving.Managers
{
    public class CheckpointManager
    {
        public LineSegment CurrentWaypoint { get; set; }

        public LineSegment LastWaypoint { get; set; }

        public float WaypointTolerance = 100f;

        public int CheckpointsPassed { get; set; } = 0;

        public int TrackCheckpoints { get; private set; } = 0;

        public int LapsCompleted { get; private set; } = 0;

        private List<LineSegment> waypoints;

        private Vector2f startPosition;

        public CheckpointManager()
        {
            waypoints = new List<LineSegment>();
        }

        public CheckpointManager(List<LineSegment> waypoints)
        {
            this.waypoints = waypoints;
            if (waypoints != null)
                this.CurrentWaypoint = waypoints.First();
            this.LastWaypoint = GetLastWaypoint(waypoints.IndexOf(CurrentWaypoint));
        }

        public void Initialize(Track track)
        {
            startPosition = track.StartPosition;

            waypoints.Clear();
            waypoints.AddRange(track.Checkpoints);

            TrackCheckpoints = waypoints.Count;
            CurrentWaypoint = waypoints.First();
            LastWaypoint = waypoints.Last();
        }

        public void Update(Vector2f currentPosition)
        {
            if (MathsHelper.LineToPointDistance2D(CurrentWaypoint, currentPosition) < WaypointTolerance)
            {
                SetNextWaypoint();
            }

            // Check that we have completed a full lap
            if (CheckpointsPassed >= TrackCheckpoints && 
                currentPosition.Magnitude(startPosition) < WaypointTolerance)
            {
                CheckpointsPassed = 0;
                LapsCompleted++;
            }
        }

        public bool CheckComplete(Vector2f currentPosition)
        {
            if (MathsHelper.LineToPointDistance2D(LastWaypoint, currentPosition) < WaypointTolerance)
            {
                return false;
            }

            return true;
        }

        private LineSegment GetLastWaypoint(int currentWaypointIndex)
        {
            var previousWaypointIndex = currentWaypointIndex - 2;
            if (previousWaypointIndex < 0)
            {
                previousWaypointIndex = waypoints.Count() - (1 - previousWaypointIndex);
            }

            return waypoints[previousWaypointIndex];
        }

        private void SetNextWaypoint()
        {
            CheckpointsPassed++;

            var currentIndex = waypoints.IndexOf(CurrentWaypoint);
            if (currentIndex + 1 < waypoints.Count())
            {
                CurrentWaypoint = waypoints[currentIndex + 1];
                this.LastWaypoint = GetLastWaypoint(waypoints.IndexOf(CurrentWaypoint));
            }
            else
            {
                CurrentWaypoint = waypoints.First();
                this.LastWaypoint = GetLastWaypoint(waypoints.IndexOf(CurrentWaypoint));
            }
        }
    }
}
