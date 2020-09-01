using SFML.Graphics;
using SFML.System;

namespace Shared.Menus
{
    public static class LayoutHelper
    {
        public static Vector2f GetTextOrigin(Text text, HorizontalAlignment horizontalAlignment)
        {
            var origin = new Vector2f();

            var bounds = text.GetLocalBounds();

            if (horizontalAlignment == HorizontalAlignment.Centre)
            {
                origin = new Vector2f(bounds.Width / 2, bounds.Height / 2);
            }
            else if (horizontalAlignment == HorizontalAlignment.Right)
            {
                origin = new Vector2f(bounds.Width, bounds.Height / 2);
            }

            return origin;
        }
    }
}
