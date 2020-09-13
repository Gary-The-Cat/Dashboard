using SFML.System;
using System.Collections.Generic;

namespace SelfDriving.Shared
{
    public class Track
    {
        public Track()
        {
            Checkpoints = new List<LineSegment>();

            Map = new List<LineSegment>();
        }

        public string FileLocation { get; set; }

        public List<LineSegment> Checkpoints { get; set; }

        public List<LineSegment> Map { get; set; }

        public Vector2f StartPosition { get; set; }

        public float InitialHeading { get; set; }
    }
}
