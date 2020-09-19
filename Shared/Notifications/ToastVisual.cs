using SFML.Graphics;
using SFML.System;

namespace Shared.Notifications
{
    public class ToastVisual
    {
        private static Vector2f ColouredRegionSize = new Vector2f(80, 50);
        private static float WidthBuffer = 100;

        private RectangleShape whiteBackground;

        private RectangleShape colouredBackground;

        private RectangleShape icon;

        private Text message;

        public ToastVisual(Toast toast)
        {
            this.message = new Text(message, new Font("Resources\\font.ttf"));
            var width = this.message.GetLocalBounds().Width;
            this.whiteBackground = new RectangleShape(new Vector2f(width + WidthBuffer, ColouredRegionSize.Y));
        }

        public void OnUpdate(float deltaT)
        {

        }

        public void OnRender(RenderTarget target)
        {
            
        }
    }
}