using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfDriving.Shared
{
    public class TrackVisual
    {
        private List<Vertex[]> visual;

        public TrackVisual(Track track)
        {
            visual = new List<Vertex[]>();

            foreach (var segment in track.Map)
            {
                var segmentVertices = new Vertex[2];
                segmentVertices[0] = new Vertex(segment.Start) { Color = Color.Black };
                segmentVertices[1] = new Vertex(segment.End) { Color = Color.Black };
                visual.Add(segmentVertices);
            }
        }

        public void OnRender(RenderTarget target)
        {
            visual.ForEach(r => target.Draw(r, 0, 2, PrimitiveType.Lines));
        }
    }
}
