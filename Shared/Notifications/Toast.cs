using SFML.Graphics;
using SFML.System;
using Shared.Interfaces;
using Shared.Maths;

namespace Shared.Notifications
{
    public class Toast : IVisual
    {
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

            finalPosition = new Vector2f(1280, 720) - visual.GetSize();
            initialPosition = new Vector2f(1280, finalPosition.Y);
            Position = initialPosition;

            worker = new EasingWorker(
                Easings.EaseInOutCirc,
                value =>
                {
                    Position = new Vector2f((float)(1280 - value), finalPosition.Y);
                },
                1.5f,
                0,
                visual.GetSize().X);
        }

        public void OnUpdate(float deltaT)
        {
            if (timeAlive > 3.4f && !easeOutStarted)
            {
                worker = new EasingWorker(
                    Easings.EaseInOutCirc,
                    value =>
                    {
                        Position = new Vector2f((float)((1280 - visual.GetSize().X) + value), finalPosition.Y);
                    },
                    1.5f,
                    0,
                    visual.GetSize().X);

                easeOutStarted = true;
            }

            worker.OnUpdate(deltaT);

            timeAlive += deltaT;

            if (IsAlive)
            {
                visual.OnUpdate(deltaT);
            }
        }

        public void OnRender(RenderTarget target)
        {
            visual.OnRender(target);
        }
    }
}
