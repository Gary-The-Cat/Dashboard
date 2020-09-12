using SFML.System;
using Shared.Commands.Interfaces;
using System;

namespace SelfDriving.Screens.MapMaker.Commands
{
    public class AddSegmentCommand : IUndoableCommand
    {
        private Guid segmentId;

        private (Vector2f start, Vector2f end) segment;

        private Func<Vector2f, Vector2f, Guid> addSegment;

        private Func<Guid, bool> removeSegment;

        private Func<Guid, (Vector2f start, Vector2f end)> getSegment;

        public AddSegmentCommand(
            (Vector2f start, Vector2f end) segment,
            Func<Vector2f, Vector2f, Guid> addSegment,
            Func<Guid, bool> removeSegment,
            Func<Guid, (Vector2f start, Vector2f end)> getSegment)
        {
            this.segment = segment;
            this.addSegment = addSegment;
            this.removeSegment = removeSegment;
            this.getSegment = getSegment;
        }

        public Guid GetSegmentId()
        {
            return segmentId;
        }

        public void UpdateSegment(Vector2f start, Vector2f end)
        {
            segment = (start, end);
        }

        public bool Execute()
        {
            segmentId = addSegment(segment.start, segment.end);
            return true;
        }

        public bool Redo()
        {
            segmentId = addSegment(segment.start, segment.end);
            return true;
        }

        public bool Undo()
        {
            segment = getSegment(segmentId);
            return removeSegment(segmentId);
        }
    }
}
