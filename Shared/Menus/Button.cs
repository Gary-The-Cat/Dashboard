using SFML.Graphics;
using SFML.System;
using Shared.ExtensionMethods;

namespace Shared.Menus
{
    public class Button
    {
        private Font font;

        private Text text;

        private RectangleShape background;

        private static Vector2f Buffer = new Vector2f(20, 10);

        public Button(string text, Vector2f position)
        {
            font = new Font("Resources\\font.ttf");
            this.text = new Text(text, font)
            {
                Position = position
            };

            var textSize = this.text.GetLocalBounds();
            var textSizeVector = new Vector2f(textSize.Width, textSize.Height);

            background = new RectangleShape(textSizeVector + Buffer)
            {
                Position = new Vector2f(position.X, position.Y) - (Buffer / 2) - new Vector2f(textSizeVector.X / 2, 0),
                FillColor = new Color(32, 126, 160),
            };
        }

        public Vector2f Position { get; set; }

        public string Text
        {
            get => text?.ToString();
            set => text = new Text(value, font) { Position = Position };
        }

        public void OnRender(RenderTarget target)
        {
            target.Draw(background);
            target.DrawString(text);
        }
    }
}
