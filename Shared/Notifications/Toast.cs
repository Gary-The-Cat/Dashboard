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

        public float Duration { get; }

        public bool IsAlive => timeAlive < Duration;

        public Vector2f Position { get; private set; }

        private Vector2f finalPosition;

        private Vector2f initialPosition;

        private float timeAlive;

        private ToastVisual visual;

        private bool easeOutStarted = false;

        private EasingWorker worker;

        public Toast(ToastType type, string message, float duration = 5)
        {
            this.Message = message;
            this.Type = type;
            this.Duration = duration;

            timeAlive = 0;
            visual = new ToastVisual(this);            
        }

        private void SetWorker(float start, float finish)
        {
            worker = new EasingWorker(
                Easings.EaseInOutCirc,
                value =>
                {
                    Position = new Vector2f((float)(value), finalPosition.Y);
                },
                TransitionTime,
                start,
                finish);
        }

        public void SetEndPosition(Vector2f vector2f)
        {
            finalPosition = vector2f;
            SetWorker(initialPosition.X, finalPosition.X);
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
                worker = null;

                return;
            }

            if (timeAlive > Duration - TransitionTime && !easeOutStarted)
            {
                SetWorker(finalPosition.X, initialPosition.X);

                easeOutStarted = true;
            }

            worker.OnUpdate(deltaT);

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
