using SelfDriving.DataStructures;
using SelfDriving.Screens.MapMaker.Commands;
using SelfDriving.Shared;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.CameraTools;
using Shared.Commands;
using Shared.Core;
using Shared.Events.CallbackArgs;
using Shared.Events.EventArgs;
using Shared.ExtensionMethods;
using Shared.Helpers;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using static SFML.Window.Mouse;

namespace SelfDriving.Screens.MapMaker
{
    public class MapMakerWorldScreen : Screen
    {
        private RectangleShape carForScale;

        private CircleShape pointHighlight;

        private ConvexShape lineHighlight;

        private MapMakerDataContainer sharedContainer;

        private CommandManager commandManager;

        private IApplication application;

        private Vector2f size;

        private Vector2f OffScreen = new Vector2f(float.MinValue, float.MinValue);

        private bool isDrawing;

        private Guid currentSegmentId;

        private bool isMoving;

        private bool isPlacingStartPosition;

        private List<(Guid, int)> currentVertices;

        private MoveVertexCommand currentMoveVertexCommand;

        private AddSegmentCommand currentAddVertexCommand;

        private Func<Vector2f, Vector2f, Guid> addTrackSegment;

        private Func<Vector2f, Vector2f, Guid> addCheckpointSegment;

        private Func<Guid, bool> removeSegment;

        private Func<Guid, (Vector2f start, Vector2f end)> getSegment;

        private const double SnapThreshold = 20;

        public MapMakerWorldScreen(
            IApplication application,
            IApplicationInstance applicationInstance,
            MapMakerDataContainer sharedContainer) 
            : base(application, applicationInstance)
        {
            this.application = application;
            this.sharedContainer = sharedContainer;

            commandManager = new CommandManager();

            this.isDrawing = false;
            this.isMoving = false;

            RegisterCallbacks();

            var carConfig = new CarConfiguration();

            carForScale = new RectangleShape(carConfig.CarSize)
            {
                FillColor = Color.White,
                OutlineColor = Color.Black,
                OutlineThickness = 2,
                Origin = carConfig.CarSize / 2
            };

            pointHighlight = new CircleShape(5)
            {
                Position = OffScreen,
                FillColor = Color.Green,
                OutlineColor = Color.Black,
                OutlineThickness = 2,
                Origin = new Vector2f(5, 5)
            };

            lineHighlight = new ConvexShape()
            {
                Position = OffScreen,
                FillColor = Color.Green,
                OutlineColor = Color.Black,
                OutlineThickness = 2,
            };

            addTrackSegment = (Vector2f start, Vector2f end) =>
            {
                return sharedContainer.AddSegment(start, end, isTrack: true);
            };

            addCheckpointSegment = (Vector2f start, Vector2f end) =>
            {
                return sharedContainer.AddSegment(start, end, isTrack: false);
            };

            removeSegment = (Guid segmentId) =>
            {
                return sharedContainer.RemoveSegment(segmentId);
            };

            getSegment = (Guid segmentId) =>
            {
                return sharedContainer.GetSegment(segmentId);
            };

            var windowSize = application.Window.Size;
            this.size = new Vector2f(windowSize.X, windowSize.Y);
            carForScale.Position = size / 2;
        }

        private void RegisterCallbacks()
        {
            RegisterMouseMoveCallback(OnMouseMove);
            RegisterMouseClickCallback(new MouseClickCallbackEventArgs(Button.Left), OnMouseClick);
            RegisterKeyboardCallback(new KeyPressCallbackEventArgs(Keyboard.Key.Z, isCtrlRequired: true), OnUndo);
            RegisterKeyboardCallback(new KeyPressCallbackEventArgs(Keyboard.Key.Y, isCtrlRequired: true), OnRedo);
        }

        public void Initialize(Track track)
        {
            sharedContainer.Clear();
            isDrawing = false;
            isMoving = false;
            isPlacingStartPosition = false;

            foreach (var segment in track.Map)
            {
                sharedContainer.AddSegment(segment.Start, segment.End, isTrack: true);
            }

            foreach (var segment in track.Checkpoints)
            {
                sharedContainer.AddSegment(segment.Start, segment.End, isTrack: false);
            }

            carForScale.Position = track.StartPosition;
            carForScale.Rotation = track.InitialHeading;
        }

        public override void OnRender(RenderTarget target)
        {
            target.SetView(Camera.GetView());

            target.Draw(carForScale);

            target.Draw(pointHighlight);

            target.Draw(lineHighlight);

            foreach (var segment in sharedContainer.trackSegments.Values)
            {
                target.Draw(segment, 0, 2, PrimitiveType.Lines);
            }
        }

        public override void OnUpdate(float deltaT)
        {
            base.OnUpdate(deltaT);
        }

        private void OnUndo(KeyboardEventArgs args)
        {
            commandManager.Undo();

            args.IsHandled = true;
        }

        private void OnRedo(KeyboardEventArgs args)
        {
            commandManager.Redo();

            args.IsHandled = true;
        }

        private void OnMouseClick(MouseClickEventArgs args)
        {
            var point = GetWorldPosition(args.Args.X, args.Args.Y, Camera);

            switch (sharedContainer.EditState)
            {
                case MapEditState.DrawingLines:
                    ProcessDrawClick(point);
                    break;
                case MapEditState.Checkpoint:
                    ProcessDrawCheckpointClick(point);
                    break;
                case MapEditState.MovingPoints:
                    ProcessMoveClick(point);
                    break;
                case MapEditState.Deletion:
                    ProcessDeleteClick(point);
                    break;
                case MapEditState.StartPosition:
                    ProcessPlacingStartPosition(point);
                    break;
            }
        }

        private void ProcessPlacingStartPosition(Vector2f point)
        {
            if (isPlacingStartPosition)
            {
                isPlacingStartPosition = false;
            }
            else
            {
                carForScale.Position = point;
                sharedContainer.StartPosition = point;
                isPlacingStartPosition = true;
            }
        }

        private void ProcessDrawClick(Vector2f point)
        {
            if (!isDrawing)
            {
                var (nearestPoint, distance) = sharedContainer.GetNearestPoint(point, isDrawing);
                var startPoint = distance < SnapThreshold ? nearestPoint.Value : point;
                var command = new AddSegmentCommand(
                    (startPoint, point),
                    addTrackSegment,
                    removeSegment,
                    getSegment);

                commandManager.ExecuteCommand(command);
                currentAddVertexCommand = command;
                currentSegmentId = command.GetSegmentId();
                isDrawing = true;
            }
            else
            {
                isDrawing = false;
                currentAddVertexCommand = null;
                currentSegmentId = Guid.Empty;
            }
        }

        private void ProcessDrawCheckpointClick(Vector2f point)
        {
            if (!isDrawing)
            {
                var (nearestPoint, distance) = sharedContainer.GetNearestPoint(point, isDrawing);
                var startPoint = distance < SnapThreshold ? nearestPoint.Value : point;
                var command = new AddSegmentCommand(
                    (startPoint, point),
                    addCheckpointSegment,
                    removeSegment,
                    getSegment);

                commandManager.ExecuteCommand(command);

                currentSegmentId = command.GetSegmentId();
                currentAddVertexCommand = command;

                isDrawing = true;
            }
            else
            {
                var newSegmentPositions = sharedContainer.TrimSegment(currentSegmentId);
                currentAddVertexCommand.UpdateSegment(newSegmentPositions.start, newSegmentPositions.end);

                isDrawing = false;
                currentAddVertexCommand = null;
                currentSegmentId = Guid.Empty;
            }
        }

        private void ProcessMoveClick(Vector2f point)
        {
            if (isMoving)
            {
                commandManager.ExecuteCommand(currentMoveVertexCommand);
                currentMoveVertexCommand = null;
                currentVertices = null;
                isMoving = false;
            }
            else
            {
                var (nearestPoint, distance) = sharedContainer.GetNearestPoint(point, isDrawing);

                if(distance > SnapThreshold)
                {
                    nearestPoint = point;
                }

                currentVertices = sharedContainer.GetSegmentsContaining(nearestPoint.Value);
                currentMoveVertexCommand = new MoveVertexCommand(currentVertices, nearestPoint.Value, sharedContainer);

                isMoving = true;
            }
        }

        private void ProcessDeleteClick(Vector2f point)
        {
            var segmentsToRemove = new List<Guid>();
            var (nearestPoint, distance) = sharedContainer.GetNearestPoint(point, isDrawing);

            // Delete vertex
            if (distance < SnapThreshold)
            {
                segmentsToRemove.AddRange(sharedContainer.GetSegmentsContaining(nearestPoint.Value).Select(v => v.Item1));
            }
            else
            {
                var (nearestLine, lineDistance) = sharedContainer.GetNearestLine(point, isDrawing);

                // Delete line
                if (lineDistance < SnapThreshold)
                {
                    segmentsToRemove.Add(nearestLine.Value);
                }
            }

            foreach (var segment in segmentsToRemove.Select(s => (s, sharedContainer.GetSegment(s))))
            {
                var deleteCommand = new DeleteSegmentCommand(
                    addTrackSegment,
                    removeSegment,
                    segment.s,
                    segment.Item2.start,
                    segment.Item2.end);

                commandManager.ExecuteCommand(deleteCommand);
            }
        }

        private void OnMouseMove(MoveMouseEventArgs args)
        {
            var point = GetWorldPosition(args.Args.X, args.Args.Y, Camera);

            switch (sharedContainer.EditState)
            {
                case MapEditState.DrawingLines:
                    HighlightClosestPoint(point);
                    ProcessDrawMove(point);
                    break;
                case MapEditState.Checkpoint:
                    ProcessDrawCheckpointMove(point);
                    break;
                case MapEditState.MovingPoints:
                    HighlightClosestPoint(point);
                    ProcessMoveMove(point);
                    break;
                case MapEditState.Deletion:
                    HighlightClosestPointOrEdge(point);
                    break;
                case MapEditState.StartPosition:
                    ProcessStartPositionMove(point);
                    break;
            }
        }

        private void ProcessStartPositionMove(Vector2f point)
        {
            if (isPlacingStartPosition)
            {
                var directionVector = point - carForScale.Position;
                carForScale.Rotation = directionVector.GetAngleDegrees() - 90;
                sharedContainer.StartRotation = carForScale.Rotation;
            }
        }

        private void HighlightClosestPointOrEdge(Vector2f point)
        {
            var (nearestPoint, distance) = sharedContainer.GetNearestPoint(point, isDrawing);

            if (distance < SnapThreshold)
            {
                lineHighlight.Position = OffScreen;
                HighlightPoint(nearestPoint.Value, distance);
            }
            else
            {
                var (nearestLine, lineDistance) = sharedContainer.GetNearestLine(point, isDrawing);

                pointHighlight.Position = OffScreen;

                if (nearestLine != null)
                {
                    HighlightEdge(nearestLine, lineDistance);
                }
                else
                {
                    lineHighlight.Position = OffScreen;
                }
            }
        }

        private void HighlightClosestPoint(Vector2f point)
        {
            var (nearestPoint, distance) = sharedContainer.GetNearestPoint(point, isDrawing);

            if(nearestPoint != null)
            {
                HighlightPoint(nearestPoint.Value, distance);
            }
        }

        private void HighlightEdge(Guid? nearestLine, double lineDistance)
        {
            var edge = sharedContainer.GetSegment(nearestLine.Value);

            if (lineDistance < SnapThreshold)
            {
                lineHighlight = SFMLGraphicsHelper.GetLine(edge.start, edge.end, 3, Color.Green);
            }
            else
            {
                lineHighlight.Position = OffScreen;
            }
        }

        private void HighlightPoint(Vector2f point, double distance)
        {
            // Check to see if we should snap to a point
            if (distance < SnapThreshold && !(isDrawing || isMoving))
            {
                pointHighlight.Position = point;
            }
            else
            {
                pointHighlight.Position = OffScreen;
            }
        }

        private void ProcessMoveMove(Vector2f point)
        {
            if (isMoving)
            {
                currentVertices.ForEach(v => sharedContainer.SetVertexPosition(v.Item1, v.Item2, point));
                currentMoveVertexCommand.UpdateFinalPosition(point);
            }
            else
            {

            }
        }

        private void ProcessDrawMove(Vector2f point)
        {
            var (nearestPoint, distance) = sharedContainer.GetNearestPoint(point, isDrawing);

            if (isDrawing)
            {
                // Check to see if we should snap to a point
                if (distance < SnapThreshold)
                {
                    sharedContainer.SetSegmentEnd(currentSegmentId, nearestPoint.Value);
                }
                else
                {
                    sharedContainer.SetSegmentEnd(currentSegmentId, point);
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.LShift))
                {
                    var newEndPoint = GetAxisLockedEndPoint(currentSegmentId, point);

                    sharedContainer.SetSegmentEnd(currentSegmentId, newEndPoint);
                }
            }
        }

        private Vector2f GetAxisLockedEndPoint(Guid segmentId, Vector2f mousePosition)
        {
            var start = sharedContainer.GetSegment(segmentId).start;
            var angle = sharedContainer.GetSegmentAngle(segmentId) / 3.14159f * 180;
            var angleDelta = angle % 45;
            if (angleDelta < 22.5)
            {
                angle -= angleDelta;
            }
            else
            {
                angle += (45 - angleDelta);
            }

            var length = sharedContainer.GetSegmentLength(segmentId);
            var angleInRadians = angle / 180 * 3.14159f;

            var newEndPoint = new Vector2f(
                (float)(start.X + length * Math.Cos(angleInRadians)),
                (float)(start.Y + length * Math.Sin(angleInRadians)));

            return newEndPoint;
        }

        private void ProcessDrawCheckpointMove(Vector2f point)
        {
            if (isDrawing)
            {
                // Update the end point to be the position of the mouse
                sharedContainer.SetSegmentEnd(currentSegmentId, point);

                // If the user is holding the shift key, lock it to 45 degree increments
                if (Keyboard.IsKeyPressed(Keyboard.Key.LShift))
                {
                    var newEndPoint = GetAxisLockedEndPoint(currentSegmentId, point);
                    sharedContainer.SetSegmentEnd(currentSegmentId, newEndPoint);
                }
            }
         }

        private Vector2f GetWorldPosition(float x, float y, Camera camera)
        {
            var cameraView = camera.GetView();
            var topLeft = cameraView.Center - cameraView.Size / 2;
            var xFraction = x / size.X;
            var yFraction = y / size.Y;

            return new Vector2f(
                topLeft.X + (xFraction * cameraView.Size.X), 
                topLeft.Y + (yFraction * cameraView.Size.Y));
        }
    }
}
