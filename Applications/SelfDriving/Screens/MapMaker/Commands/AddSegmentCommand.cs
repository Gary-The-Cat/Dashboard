using SFML.System;
using Shared.Commands.Interfaces;
using System;

namespace SelfDriving.Screens.MapMaker.Commands
{
    public class AddSegmentCommand : IUndoableCommand
    {
        private Guid segmentId;

        private (Vector2f start, Vector2f end) segment;

        private MapMakerDataContainer container;

        public AddSegmentCommand(
            (Vector2f start, Vector2f end) segment,
            MapMakerDataContainer container)
        {
            this.segment = segment;
            this.container = container;
        }

        public Guid GetSegmentId()
        {
            return segmentId;
        }

        public bool Execute()
        {
            segmentId = container.AddTrackSegment(segment.start, segment.end);
            return true;
        }

        public bool Redo()
        {
            segmentId = container.AddTrackSegment(segment.start, segment.end);
            return true;
        }

        public bool Undo()
        {
            segment = container.GetSegment(segmentId);
            return container.RemoveSegment(segmentId);
        }
    }
}
