using SFML.Graphics;
using SFML.System;
using Shared.Interfaces;

namespace Shared.Notifications
{
    public class ToastVisual : IVisual
    {
        private static Vector2f ColoredRegionSize = new Vector2f(70, 58);
        private static Vector2f IconSize = new Vector2f(50, 50);
        private static float WidthBuffer = 40;

        private RectangleShape whiteBackground;

        private RectangleShape coloredBackground;

        private RectangleShape icon;

        private Text text;

        private Toast toast;

        public ToastVisual(Toast toast)
        {
            this.toast = toast;
            this.text = new Text(toast.Message, new Font("Resources\\font.ttf")) { FillColor = new Color(0x5e, 0x5e, 0x5e) };
            this.coloredBackground = GetColoredBackground(toast.Type);
            this.icon = GetToastIcon(toast.Type);
            this.whiteBackground = GetWhiteBackground(text);
        }

        private RectangleShape GetWhiteBackground(Text message)
        {
            var width = message.GetLocalBounds().Width;
            return new RectangleShape(new Vector2f(width + WidthBuffer, ColoredRegionSize.Y))
            {
                FillColor = Color.White
            };
        }

        private RectangleShape GetToastIcon(ToastType type)
        {
            var texture = new Texture($"Resources\\{GetToastImage(type)}");
            texture.GenerateMipmap();
            return new RectangleShape()
            {
                Size = IconSize,
                Texture = texture
            };
        }

        private RectangleShape GetColoredBackground(ToastType type)
        {
            return new RectangleShape()
            {
                Size = ColoredRegionSize,
                FillColor = GetColorFromType(type)
            };
        }

        private string GetToastImage(ToastType type)
        {
            switch (type)
            {
                case ToastType.Info:
                    return "Info.png";
                case ToastType.Successful:
                    return "Successful.png";
                case ToastType.Warning:
                    return "Warning.png";
                case ToastType.Error:
                    return "Error.png";
                default:
                    return "Error.png";
            }
        }

        private Color GetColorFromType(ToastType type)
        {
            switch (type)
            {
                case ToastType.Info:
                    return new Color(0x83, 0x83, 0x83);
                case ToastType.Successful:
                    return new Color(0x40, 0x9C, 0x35);
                case ToastType.Warning:
                    return new Color(0xEC, 0x7A, 0x07);
                case ToastType.Error:
                    return new Color(0xCC, 0x00, 0x00);
                default:
                    return new Color(0x83, 0x83, 0x83);
            }
        }

        public Vector2f GetSize()
        {
            return new Vector2f(ColoredRegionSize.X + whiteBackground.GetLocalBounds().Width, ColoredRegionSize.Y);
        }

        public void OnUpdate(float deltaT)
        {
            coloredBackground.Position = toast.Position;
            icon.Position = new Vector2f(toast.Position.X + ((ColoredRegionSize.X - IconSize.X)/2), toast.Position.Y + ((ColoredRegionSize.Y - IconSize.Y) / 2));
            whiteBackground.Position = new Vector2f(toast.Position.X + ColoredRegionSize.X, toast.Position.Y);
            var verticalBuffer = (ColoredRegionSize.Y - text.GetLocalBounds().Height) / 2;
            text.Position = new Vector2f(toast.Position.X + ColoredRegionSize.X + (WidthBuffer / 2), toast.Position.Y + (verticalBuffer)/2);
        }

        public void OnRender(RenderTarget target)
        {
            target.Draw(whiteBackground);
            target.Draw(coloredBackground);
            target.Draw(icon);
            target.Draw(text);
        }
    }
}