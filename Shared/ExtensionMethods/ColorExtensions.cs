using SFML.Graphics;

namespace Shared.ExtensionMethods
{
    public static class ColorExtensions
    {
        public static Color Darken(this Color color, double amount)
        {
            var colour = new Color();
            colour.R = color.R < amount ? (byte)0 : (byte)(color.R - amount);
            colour.G = color.G < amount ? (byte)0 : (byte)(color.G - amount);
            colour.B = color.B < amount ? (byte)0 : (byte)(color.B - amount);
            colour.A = byte.MaxValue;

            return colour;
        }
    }
}
