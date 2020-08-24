using SFML.Graphics;
using SFML.System;
using Shared.Graphing;

namespace Shared.ExtensionMethods
{
    public static class RenderWindowExtensions
    {
        public static void Draw(this RenderTarget target, Texture texture, IntRect region, Vector2f position, float rotation = 0.0f, float scale = 1.0f)
        {
            Sprite sprite = new Sprite(texture, region);
            Draw(target, sprite, position, rotation, scale);
        }

        public static void Draw(this RenderTarget target, Texture texture, Vector2f position, float rotation = 0.0f, float scale = 1.0f)
        {
            IntRect region = new IntRect(0, 0, (int)texture.Size.X, (int)texture.Size.X);
            Draw(target, texture, region, position, rotation, scale);
        }

        public static void Draw(this RenderTarget target, Sprite sprite, Vector2f position, float rotation = 0.0f, float scale = 1.0f)
        {
            sprite.Position = position;
            sprite.Rotation = rotation;
            sprite.Scale = new Vector2f(scale, scale);
            target.Draw(sprite);
        }

        public static void DrawString(this RenderTarget target, Vector2f position, string text, Font font, uint size, Color color, bool centre = true)
        {
            Text sprite = new Text(text, font, size)
            {
                Position = position,
                FillColor = color
            };

            var bounds = sprite.GetLocalBounds();
            sprite.Origin = new Vector2f(bounds.Width / 2, bounds.Height / 2);
            target.Draw(sprite);
        }

        public static void Draw(this RenderTarget target, CartesianGraph graph)
        {
            graph.Draw(target);
        }
    }
}
