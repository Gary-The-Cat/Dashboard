using SFML.Graphics;

namespace Shared.ExtensionMethods
{
    public static class ColorExtensions
    {
        public static Color Darken(this Color color, double amount)
        {
            var darkenedColor = new Color();
            darkenedColor.R = color.R < amount ? (byte)0 : (byte)(color.R - amount);
            darkenedColor.G = color.G < amount ? (byte)0 : (byte)(color.G - amount);
            darkenedColor.B = color.B < amount ? (byte)0 : (byte)(color.B - amount);
            darkenedColor.A = byte.MaxValue;

            return darkenedColor;
        }
    }
}
