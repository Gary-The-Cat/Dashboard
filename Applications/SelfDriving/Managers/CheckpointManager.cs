using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SelfDriving.Managers
{
    public class CheckpointManager
    {
        public Vector2f CurrentWaypoint { get; set; }

        public Vector2f LastWaypoint { get; set; }

        public float WaypointTolerance = 100f;

        public int CheckpointsPassed { get; set; } = 0;

        private List<Vector2f> waypoints;

        public CheckpointManager()
        {
            waypoints = new List<Vector2f>();
        }

        public CheckpointManager(List<Vector2f> waypoints)
        {
            this.waypoints = waypoints;
            if (waypoints != null)
                this.CurrentWaypoint = waypoints.First();
            this.LastWaypoint = GetLastWaypoint(waypoints.IndexOf(CurrentWaypoint));
        }

        public void Initialize(List<Vector2f> waypoints)
        {
            this.waypoints.Clear();
            this.waypoints.AddRange(waypoints);
            this.CurrentWaypoint = this.waypoints.First();
            this.LastWaypoint = this.waypoints.Last();
        }

        public void Update(Vector2f currentPosition)
        {
            if (GetDistance(CurrentWaypoint, currentPosition) < WaypointTolerance)
            {
                SetNextWaypoint();
            }
        }

        public bool CheckComplete(Vector2f currentPosition)
        {
            if (GetDistance(LastWaypoint, currentPosition) < WaypointTolerance)
            {
                return false;
            }

            return true;
        }

        private Vector2f GetLastWaypoint(int currentWaypointIndex)
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


        private float GetDistance(Vector2f a, Vector2f b)
        {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
    }
}
