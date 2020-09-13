using System.Collections.Generic;

namespace SelfDriving.Shared
{
    /// <summary>
    /// This class is used only to convert old maps into the new format and
    /// is just copied and pasted values from old files.
    /// </summary>
    public static class MapStructure
    {
        public static Track GetTrack()
        {
            var track = new Track();

            track.Map = new List<LineSegment>()
            {
                new LineSegment(59,1013,60,49       ),
                new LineSegment(60,49,1326,48       ),
                new LineSegment(1326,48,1325,406    ),
                new LineSegment(1325,406,743,407    ),
                new LineSegment(743,407,743,449     ),
                new LineSegment(743,449,1847,449    ),
                new LineSegment(1847,449,1847,1013  ),
                new LineSegment(1847,1013,1067,1013 ),
                new LineSegment(1067,1013,989,1061  ),
                new LineSegment(989,1061,371,1061   ),
                new LineSegment(371,1061,287,1013   ),
                new LineSegment(287,1013,59,1013    ),
                new LineSegment(1109,209,1109,252   ),
                new LineSegment(1109,252,550,253    ),
                new LineSegment(550,253,551,605     ),
                new LineSegment(551,605,1655,605    ),
                new LineSegment(1655,605,1655,839   ),
                new LineSegment(1655,839,1067,839   ),
                new LineSegment(1067,839,977,791    ),
                new LineSegment(977,791,371,791     ),
                new LineSegment(371,791,293,857     ),
                new LineSegment(293,857,221,857     ),
                new LineSegment(221,857,221,209     ),
                new LineSegment(221,209,1109,209    ),
                new LineSegment(395,917,653,899     ),
                new LineSegment(653,899,923,917     ),
                new LineSegment(923,917,659,935     ),
                new LineSegment(659,935,395,917     ),
            };

            track.StartPosition = new SFML.System.Vector2f(137, 185);
            track.InitialHeading = 90;
            track.Checkpoints = new List<LineSegment>();

            return track;
        }
    }
}
