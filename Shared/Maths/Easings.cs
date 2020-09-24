using System;

namespace Shared.Maths
{
    public static class Easings
    {
        // #ref https://easings.net/#easeInOutSine
        public static double EaseInOutSine(double x)
        {
            return -(Math.Cos(Math.PI * x) - 1) / 2;
        }

        // #ref https://easings.net/#easeInOutCubic
        public static double EaseInOutCubic(double x)
        {
            return x < 0.5 ? 4 * x * x * x : 1 - Math.Pow(-2 * x + 2, 3) / 2;
        }

        // #ref https://easings.net/#easeInOutQuint
        public static double EaseInOutQuint(double x)
        {
            return x < 0.5 ? 16 * x * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 5) / 2;
        }

        // #ref https://easings.net/#easeInOutCirc
        public static double EaseInOutCirc(double x)
        {
            return x < 0.5
              ? (1 - Math.Sqrt(1 - Math.Pow(2 * x, 2))) / 2
              : (Math.Sqrt(1 - Math.Pow(-2 * x + 2, 2)) + 1) / 2;
        }

        // #ref https://easings.net/#easeInOutBack
        public static double EaseInOutBack(double x)
        {
            const double c1 = 1.70158;
            const double c2 = c1 * 1.525;

            return x < 0.5
              ? (Math.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
              : (Math.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
        }

        // #ref https://easings.net/#easeInBack
        public static double EaseInBack(double x)
        {
            const double c1 = 1.70158;
            const double c3 = c1 + 1;

            return c3 * x * x * x - c1 * x * x;
        }

        // #ref https://easings.net/#easeOutBack
        public static double EaseOutBack(double x)
        {
            const double c1 = 1.70158;
            const double c3 = c1 + 1;

            return 1 + c3 * Math.Pow(x - 1, 3) + c1 * Math.Pow(x - 1, 2);
        }

        // #ref https://easings.net/#easeOutQuint
        public static double EaseOutQuint(double x)
        {
            return 1 - Math.Pow(1 - x, 5);
        }
    }
}
