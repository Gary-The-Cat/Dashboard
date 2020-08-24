using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Controllers
{
    public static class ButtonTransforms
    {
        private static float NullZone = 3277;

        public static (XBoxButton, float) TransformX(int value)
        {
            var button = XBoxButton.X;

            var wrappedValue = (value - 32768);

            var absValue = Math.Abs(wrappedValue);

            float mappedValue = absValue < NullZone ? 0 : wrappedValue / 32768f;

            return (button, mappedValue);
        }

        public static (XBoxButton, float) TransformY(int value)
        {
            var button = XBoxButton.Y;
            float mappedValue = (value - 32768) / 65536f;

            return (button, mappedValue);
        }

        public static (XBoxButton, float) TransformZ(int value)
        {
            var button = value < 32768 ? XBoxButton.RT : XBoxButton.LT;
            float mappedValue;

            if (button == XBoxButton.RT)
            {
                mappedValue = 0.9999695f - value / 32768.0f;
            }
            else
            {
                mappedValue = (value - 32768) / 32768.0f;
            }

            return (button, mappedValue);
        }
    }
}
