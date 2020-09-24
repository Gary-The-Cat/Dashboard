using SFML.Graphics;
using SFML.System;
using Shared.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Notifications
{
    public class NotificationService : INotificationService
    {
        private Dictionary<ScreenLocation, List<Toast>> toastLookup;

        private IApplication application;

        private Vector2f toastBuffer = new Vector2f(20, 20);

        public NotificationService(IApplication application)
        {
            this.application = application;

            toastLookup = new Dictionary<ScreenLocation, List<Toast>>();
            toastLookup.Add(ScreenLocation.BottomLeft, new List<Toast>());
            toastLookup.Add(ScreenLocation.BottomRight, new List<Toast>());
            toastLookup.Add(ScreenLocation.TopLeft, new List<Toast>());
            toastLookup.Add(ScreenLocation.TopRight, new List<Toast>());
        }

        public void OnRender(RenderTarget target)
        {
            foreach (var toastVisuals in toastLookup.Values)
            {
                foreach (var toastVisual in toastVisuals)
                {
                    toastVisual.OnRender(target);
                }

                toastVisuals.RemoveAll(t => !t.IsAlive);
            }
        }

        public void OnUpdate(float deltaT)
        {
            foreach (var toastVisuals in toastLookup.Values)
            {
                foreach (var toastVisual in toastVisuals)
                {
                    toastVisual.OnUpdate(deltaT);
                }
            }
        }

        public void ShowToast(ToastType type, string message)
        {
            ShowToast(ScreenLocation.BottomRight, type, message);
        }

        public void ShowToast(ScreenLocation location, ToastType type, string message)
        {
            var toast = new Toast(type, location, message);
            var size = toast.GetSize();
            toast.SetStartPosition(GetStartPosition(location, size));
            toast.SetDesiredXPosition(toast.Position.X, GetEndPosition(location, size).X);

            toastLookup[location].Add(toast);

            RefreshToastVisuals(location);
        }

        private void RefreshToastVisuals(ScreenLocation location)
        {
            var reversedList = toastLookup[location].ToList();
            reversedList.Reverse();

            foreach (var toast in reversedList)
            {
                var index = reversedList.IndexOf(toast);
                var yPositionFromIndex = GetYPositionFromIndex(toast.Location, index);
                toast.SetDesiredYPosition(toast.Position.Y, yPositionFromIndex, 1);
            }
        }

        private float GetYPositionFromIndex(ScreenLocation location, int index)
        {
            var windowSize = application.Window.Size;

            switch (location)
            {
                case ScreenLocation.BottomRight:
                    return windowSize.Y - ((toastBuffer.Y + ToastVisual.ColoredRegionSize.Y) * (index + 1));
                case ScreenLocation.TopRight:
                    return toastBuffer.Y + (toastBuffer.Y + ToastVisual.ColoredRegionSize.Y) * (index);
                case ScreenLocation.BottomLeft:
                    return windowSize.Y - ((toastBuffer.Y + ToastVisual.ColoredRegionSize.Y) * (index + 1));
                case ScreenLocation.TopLeft:
                    return toastBuffer.Y + (toastBuffer.Y + ToastVisual.ColoredRegionSize.Y) * (index);
            }

            return ToastVisual.ColoredRegionSize.Y + (toastBuffer.Y + ToastVisual.ColoredRegionSize.Y) * index;
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
            float x = 0;
            float y = 0;

            switch (location)
            {
                case ScreenLocation.TopLeft:
                    x = -size.X;
                    y = toastBuffer.Y;
                    break;
                case ScreenLocation.BottomLeft:
                    x = -size.X;
                    y = windowSize.Y - toastBuffer.Y;
                    break;
                case ScreenLocation.TopRight:
                    x = windowSize.X;
                    y = toastBuffer.Y;
                    break;
                case ScreenLocation.BottomRight:
                    x = windowSize.X;
                    y = windowSize.Y - toastBuffer.Y;
                    break;
            }

            return new Vector2f(x, y);
        }
    }
}
