using SFML.Graphics;
using SFML.System;

namespace Shared.ExtensionMethods
{
    public static class RectangleShapeExtensions
    {
        public static bool IsInsideShape(this RectangleShape shape, Vector2i point)
        {
            return shape.GetGlobalBounds().Contains(point.X, point.Y);
        }
    }
}
