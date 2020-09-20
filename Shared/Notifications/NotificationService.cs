using SFML.Graphics;
using SFML.System;
using Shared.Interfaces;
using System;
using System.Collections.Generic;

namespace Shared.Notifications
{
    public class NotificationService : INotificationService
    {
        private List<Toast> toastVisuals;

        private IApplication application;

        public NotificationService(IApplication application)
        {
            this.application = application;

            toastVisuals = new List<Toast>();
        }

        public void OnRender(RenderTarget target)
        {
            toastVisuals.ForEach(t => t.OnRender(target));
            toastVisuals.RemoveAll(t => !t.IsAlive);
        }

        public void OnUpdate(float deltaT)
        {
            toastVisuals.ForEach(t => t.OnUpdate(deltaT));
        }

        public void ShowToast(ToastType type, string message)
        {
            ShowToast(ScreenLocation.BottomRight, type, message);
        }

        public void ShowToast(ScreenLocation location, ToastType type, string message)
        {
            var toast = new Toast(type, message);
            var size = toast.GetSize();
            toast.SetStartPosition(GetStartPosition(location, size));
            toast.SetEndPosition(GetEndPosition(location, size));

            toastVisuals.Add(toast);
        }

        private Vector2f GetEndPosition(ScreenLocation location, Vector2f size)
        {
            var windowSize = application.Window.Size;
            var buffer = new Vector2f(20, 20);
            float x = 0;
            float y = 0;

            switch (location)
            {
                case ScreenLocation.TopLeft:
                    x = buffer.X;
                    y = buffer.Y;
                    break;
                case ScreenLocation.BottomLeft:
                    x = buffer.X;
                    y = windowSize.Y - size.Y - buffer.Y;
                    break;
                case ScreenLocation.TopRight:
                    x = windowSize.X - size.X - buffer.X;
                    y = buffer.Y;
                    break;
                case ScreenLocation.BottomRight:
                    x = windowSize.X - size.X - buffer.X;
                    y = windowSize.Y - size.Y - buffer.Y;
                    break;
            }

            return new Vector2f(x, y);
        }

        private Vector2f GetStartPosition(ScreenLocation location, Vector2f size)
        {
            var windowSize = application.Window.Size;
            var buffer = new Vector2f(20, 20);
            float x = 0;
            float y = 0;

            switch (location)
            {
                case ScreenLocation.TopLeft:
                    x = -size.X;
                    y = buffer.Y;
                    break;
                case ScreenLocation.BottomLeft:
                    x = -size.X;
                    y = windowSize.Y - buffer.Y;
                    break;
                case ScreenLocation.TopRight:
                    x = windowSize.X;
                    y = buffer.Y;
                    break;
                case ScreenLocation.BottomRight:
                    x = windowSize.X;
                    y = windowSize.Y - buffer.Y;
                    break;
            }

            return new Vector2f(x, y);
        }
    }
}
