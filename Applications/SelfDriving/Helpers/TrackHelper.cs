using Newtonsoft.Json;
using SelfDriving.Shared;
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
                tracks.Add(track);
            }

            return tracks;
        }
    }
}
