using Newtonsoft.Json;
using SFML.System;
namespace SelfDriving.Shared
{
    public struct LineSegment
    {
        [JsonProperty("Start")]
        public Vector2f Start { get; }

        [JsonProperty("End")]
        public Vector2f End { get; }

        public LineSegment(float x1, float y1, float x2, float y2)
        {
            Start = new Vector2f(x1, y1);
            End = new Vector2f(x2, y2);
        }

        public LineSegment(Vector2f start, Vector2f end)
        {
            Start = start;
            End = end;
        }
    }
}
