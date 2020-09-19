using SFML.Graphics;
using SFML.System;
using Shared.Helpers;
using System;
using System.Linq;

namespace SelfDriving.Shared
{
    public static class ThumbnailHelper
    {
        public static void GenerateTrackThumbnail(Track track, string saveLocation)
        {
            var texture = new RenderTexture(1024, 1024);

            texture.Clear(new Color(31, 126, 160));

            var xMax = Math.Max(track.Map.Max(t => t.Start.X), track.Map.Max(t => t.End.X));
            var xMin = Math.Min(track.Map.Min(t => t.Start.X), track.Map.Min(t => t.End.X));
            var yMax = Math.Max(track.Map.Max(t => t.Start.Y), track.Map.Max(t => t.End.Y));
            var yMin = Math.Min(track.Map.Min(t => t.Start.Y), track.Map.Min(t => t.End.Y));

            var deltaX = xMax - xMin;
            var deltaY = yMax - yMin;

            var delta = Math.Max(deltaX, deltaY);
            var size = new Vector2f(delta + 100, delta + 100);
            var centre = new Vector2f((xMax + xMin) / 2, (yMax + yMin) / 2);

            texture.SetView(new View(centre, size));

            foreach (var segment in track.Map)
            {
                var segmentVisual = SFMLGraphicsHelper.GetLine(segment.Start, segment.End, 16, Color.White);
                texture.Draw(segmentVisual);
            }

            texture.GenerateMipmap();
            texture.Display();

            texture.Texture.CopyToImage().SaveToFile(saveLocation);
        }
    }
}
