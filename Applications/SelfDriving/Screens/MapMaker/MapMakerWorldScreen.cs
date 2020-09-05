using SelfDriving.DataStructures;
using SelfDriving.Shared;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.CameraTools;
using Shared.Commands;
using Shared.Core;
using Shared.Interfaces;
using System;

namespace SelfDriving.Screens.MapMaker
{
    public class MapMakerWorldScreen : Screen
    {
        private RectangleShape carForScale;

        private CircleShape pointHighlight;

        private MapMakerDataContainer sharedContainer;

        private CommandManager commandManager;

        private IApplication application;

        private Vector2f size;

        private Vector2f OffScreen = new Vector2f(float.MinValue, float.MinValue);

        private bool isDrawing;

        private Guid currentSegmentId;

        private bool isMoving;

        public MapMakerWorldScreen(
            IApplication application,
            MapMakerDataContainer sharedContainer) : base(application.Configuration)
        {
            this.application = application;
            this.sharedContainer = sharedContainer;

            commandManager = new CommandManager();

            this.isDrawing = false;
            this.isMoving = false;

            RegisterMouseMoveCallback(application.Window, OnMouseMove);
            RegisterMouseClickCallback(application.Window, Mouse.Button.Left, OnMouseClick);

            var carConfig = new CarConfiguration();

            carForScale = new RectangleShape(carConfig.CarSize)
            {
                FillColor = Color.White,
                OutlineColor = Color.Black,
                OutlineThickness = 2
            };

            pointHighlight = new CircleShape(5)
            {
                Position = OffScreen,
                FillColor = Color.Green,
                OutlineColor = Color.Black,
                OutlineThickness = 2,
                Origin = new Vector2f(5, 5)
            };

            var windowSize = application.Window.Size;
            this.size = new Vector2f(windowSize.X, windowSize.Y);
            carForScale.Position = size / 2;
        }

        public void Initialize(Track track)
        {
            foreach (var segment in track.Map)
            {
                sharedContainer.AddTrackSegment(segment.Start, segment.End);
            }
        }

        public override void OnRender(RenderTarget target)
        {
            base.OnRender(target);

            target.Draw(carForScale);

            target.Draw(pointHighlight);

            foreach (var segment in sharedContainer.trackSegments.Values)
            {
                target.Draw(segment, 0, 2, PrimitiveType.Lines);
            }
        }

        public override void OnUpdate(float deltaT)
        {
            base.OnUpdate(deltaT);
        }

        private void OnMouseClick(float x, float y)
        {
            var point = GetWorldPosition(x, y, Camera);

            switch (sharedContainer.EditState)
            {
                case MapEditState.DrawingLines:
                    ProcessDrawClick(point);
                    break;
                case MapEditState.MovingPoints:
                    ProcessMoveClick(point);
                    break;
            }
        }

        private void ProcessDrawClick(Vector2f point)
        {
            if (!isDrawing)
            {
                var (nearestPoint, distance) = sharedContainer.GetNearestPoint(point, isDrawing);
                currentSegmentId = sharedContainer.AddTrackSegment(distance < 10 ? nearestPoint.Value : point, point);
                isDrawing = true;
            }
            else
            {
                isDrawing = false;
                currentSegmentId = Guid.Empty;
            }
        }

        private void ProcessMoveClick(Vector2f point)
        {
            if (isMoving)
            {

            }
            else
            {
                isMoving = true;
            }
        }

        private void OnMouseMove(float x, float y)
        {
            var point = GetWorldPosition(x, y, Camera);

            var (nearestPoint, distance) = sharedContainer.GetNearestPoint(point, isDrawing);

            if (isDrawing)
            {
                // Check to see if we should snap to a point
                if (distance < 10)
                {
                    sharedContainer.SetSegmentEnd(currentSegmentId, nearestPoint.Value);
                    pointHighlight.Position = nearestPoint.Value;
                }
                else
                {
                    sharedContainer.SetSegmentEnd(currentSegmentId, point);
                    pointHighlight.Position = OffScreen;
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.LShift))
                {
                    var start = sharedContainer.GetSegment(currentSegmentId).start;
                    var angle = sharedContainer.GetCurrentSegmentAngle() / 3.14159f * 180;
                    var angleDelta = angle % 45;
                    if (angleDelta < 22.5)
                    {
                        angle -= angleDelta;
                    }
                    else
                    {
                        angle +=  (45 - angleDelta);
                    }

                    var length = sharedContainer.GetCurrentSegmentLength();
                    var angleInRadians = angle / 180 * 3.14159f;

                    var newEndPoint = new Vector2f(
                        (float)(start.X + length * Math.Cos(angleInRadians)),
                        (float)(start.Y + length * Math.Sin(angleInRadians)));

                    sharedContainer.SetSegmentEnd(currentSegmentId, newEndPoint);
                }
            }
            else
            {
                if (distance < 10)
                {
                    pointHighlight.Position = nearestPoint.Value;
                }
                else
                {
                    pointHighlight.Position = OffScreen;
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
