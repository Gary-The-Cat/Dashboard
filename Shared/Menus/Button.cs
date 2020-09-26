using SFML.Graphics;
using SFML.System;
using Shared.ExtensionMethods;
using System;

namespace Shared.Menus
{
    public class Button
    {
        private Font font;

        private Text text;

        private RectangleShape background;

        private Action callback;

        private Vector2f position;

        // :TODO: Add alignment logic for vertical alignment and text alignment inside the button
        private HorizontalAlignment buttonHorizontalAlignment;

        private static Vector2f BorderBuffer = new Vector2f(30, 20);

        private static Vector2f LargeTextMargin = new Vector2f(0, 16);
        private static Vector2f SmallTextMargin = new Vector2f(0, 12);

        public Button(
            string text, 
            Vector2f position,
            Action callback,
            HorizontalAlignment buttonHorizontalAlignment)
        {
            font = new Font("Resources\\font.ttf");

            this.callback = callback;
            this.position = position;
            this.buttonHorizontalAlignment = buttonHorizontalAlignment;

            background = new RectangleShape()
            {
                FillColor = new Color(36, 142, 180),
                OutlineColor = new Color(32, 126, 160),
                OutlineThickness = 2
            };

            Text = text;

            this.SetPositions();
        }

        public string Text
        {
            get => text?.ToString();
            set 
            {
                text = new Text(value, font);
                this.text.FillColor = new Color(233, 233, 233);
                this.SetPositions();
            }
        }

        private void SetPositions()
        {
            var textSize = this.text.GetLocalBounds();
            var textSizeVector = new Vector2f(textSize.Width, textSize.Height);
            var backgroundSize = textSizeVector + BorderBuffer;
            background.Size = backgroundSize;

            background.Position = GetPosition(position, backgroundSize, buttonHorizontalAlignment);

            if(textSize.Height > 25)
            {
                text.Position = background.Position + new Vector2f(background.Size.X / 2, 0) + LargeTextMargin;
            }
            else
            {
                text.Position = background.Position + new Vector2f(background.Size.X / 2, 0) + SmallTextMargin;
            }
        }

        public void OnRender(RenderTarget target)
        {
            target.Draw(background);
            target.DrawString(text);
        }

        public Vector2f GetPosition(
            Vector2f position,
            Vector2f size,
            HorizontalAlignment horizontalAlignment)
        {
            var resultantPosition = position;

            if (horizontalAlignment == HorizontalAlignment.Centre)
            {
                resultantPosition -= new Vector2f(size.X / 2, 0);
            }
            else if (horizontalAlignment == HorizontalAlignment.Right)
            {
                resultantPosition -= new Vector2f(size.X, 0);
            }

            // Get the centre of the text
            return resultantPosition;
        }

        public FloatRect GetGlobalBounds()
        {
            return background.GetGlobalBounds();
        }

        public void TryClick(Shared.Events.EventArgs.MouseClickEventArgs eventArgs)
        {
            if (GetGlobalBounds().Contains(eventArgs.Args.X, eventArgs.Args.Y))
            {
                callback.Invoke();

                eventArgs.IsHandled = true;
            }
        }
    }
}