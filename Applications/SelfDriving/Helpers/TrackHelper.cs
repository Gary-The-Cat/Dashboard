using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SelfDriving.Shared;
using SFML.System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SelfDriving.Helpers
{
    public static class TrackHelper
    {
        public static List<Track> LoadTrackFiles(string trackDirectory)
        {
            var trackFiles = Directory.GetFiles(trackDirectory).Where(f => Path.GetExtension(f).Equals(".json"));

            var tracks = new List<Track>();
            foreach (var file in trackFiles)
            {
                var trackText = File.ReadAllText(file);
                var track = JsonConvert.DeserializeObject<Track>(trackText);
                track.FileLocation = file;
                track.Map.Clear();
                var trackObject = (JObject)JsonConvert.DeserializeObject(trackText);
                var map = trackObject["Map"];

                foreach (var line in map)
                {
                    var start = line["Start"].ToObject<Vector2f>();
                    var end = line["End"].ToObject<Vector2f>();
                    track.Map.Add(new LineSegment(start, end));
                }

                tracks.Add(track);
            }

            return tracks;
        }
    }
}
