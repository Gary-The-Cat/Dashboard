using SFML.System;
using Shared.Commands.Interfaces;
using System;

namespace SelfDriving.Screens.MapMaker.Commands
{
    public class DeleteSegmentCommand : IUndoableCommand
    {
        private Guid segmentId;
        private Vector2f start;
        private Vector2f end;
        private Func<Vector2f, Vector2f, Guid> addSegment;
        private Func<Guid, bool> removeSegment;

        public DeleteSegmentCommand(
            Func<Vector2f, Vector2f, Guid> addSegment,
            Func<Guid, bool> removeSegment,
            Guid segmentId, 
            Vector2f start, 
            Vector2f end) => 
            (this.addSegment, this.removeSegment, this.segmentId, this.start, this.end) = 
            (addSegment, removeSegment, segmentId, start, end);

        public bool Execute()
        {
            return removeSegment(segmentId);
        }

        public bool Redo()
        {
            return Execute();
        }

        public bool Undo()
        {
            segmentId = addSegment(start, end);
            return true;
        }
    }
}
