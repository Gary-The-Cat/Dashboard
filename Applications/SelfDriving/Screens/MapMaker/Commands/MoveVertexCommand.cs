using SFML.System;
using Shared.Commands.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SelfDriving.Screens.MapMaker.Commands
{
    public class MoveVertexCommand : IUndoableCommand
    {
        private Vector2f startPosition;
        private Vector2f endPosition;
        List<(Guid, int)> segments;
        MapMakerDataContainer container;

        public MoveVertexCommand(
            List<(Guid, int)> segments,
            Vector2f startPosition,
            MapMakerDataContainer container)
        {
            this.segments = segments.ToList();
            this.startPosition = startPosition;
            this.container = container;
        }

        public void UpdateFinalPosition(Vector2f endPosition)
        {
            this.endPosition = endPosition;
        }

        public bool Execute()
        {
            foreach(var segmentVertex in segments)
            {
                container.SetVertexPosition(segmentVertex.Item1, segmentVertex.Item2, endPosition);
            }

            return true;
        }

        public bool Redo()
        {
            return Execute();
        }

        public bool Undo()
        {
            foreach (var segmentVertex in segments)
            {
                container.SetVertexPosition(segmentVertex.Item1, segmentVertex.Item2, startPosition);
            }

            return true;
        }
    }
}
