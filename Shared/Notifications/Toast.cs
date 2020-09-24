using SFML.Graphics;
using SFML.System;
using Shared.Interfaces;
using Shared.Maths;
using System;

namespace Shared.Notifications
{
    public class Toast : IVisual
    {
        private static float TransitionTime = 1.5f;

        public string Message { get; }

        public ToastType Type { get; }

        public ScreenLocation Location { get; }

        public float Duration { get; }

        public bool IsAlive => timeAlive < Duration;

        public Vector2f Position { get; private set; }

        private Vector2f initialPosition { get; set; }

        private float timeAlive;

        private ToastVisual visual;

        private bool easeOutStarted = false;

        private EasingWorker xWorker;

        private EasingWorker yWorker;

        public Toast(ToastType type, ScreenLocation location, string message, float duration = 5)
        {
            this.Message = message;
            this.Type = type;
            this.Location = location;
            this.Duration = duration;

            timeAlive = 0;
            visual = new ToastVisual(this);            
        }

        public void SetDesiredXPosition(float start, float finish, float transitionTime = 1.5f)
        {
            xWorker = new EasingWorker(
                       Easings.EaseInOutCirc,
                       value =>
                       {
                           Position = new Vector2f((float)value, Position.Y);
                       },
                       transitionTime,
                       start,
                       finish);
        }

        public void SetDesiredYPosition(float start, float finish, float transitionTime = 1.5f)
        {
            yWorker = new EasingWorker(
                      Easings.EaseOutQuint,
                      value =>
                      {
                          Position = new Vector2f(Position.X, (float)value);
                      },
                      transitionTime,
                      start,
                      finish);
        }

        public void SetStartPosition(Vector2f vector2f)
        {
            initialPosition = vector2f;
            Position = vector2f;
        }

        public void OnUpdate(float deltaT)
        {
            if (!IsAlive)
            {
                xWorker = null;

                return;
            }

            if (timeAlive > Duration - TransitionTime && !easeOutStarted)
            {
                SetDesiredXPosition(Position.X, initialPosition.X);

                easeOutStarted = true;
            }

            xWorker.OnUpdate(deltaT);

            yWorker?.OnUpdate(deltaT);

            timeAlive += deltaT;

            visual.OnUpdate(deltaT);
        }

        public void OnRender(RenderTarget target)
        {
            visual.OnRender(target);
        }

        public Vector2f GetSize()
        {
            return visual.GetSize();
        }

    }
}
