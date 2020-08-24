using SFML.Graphics;
using static SFML.Window.Keyboard;

namespace Shared.ScreenConfig
{
    public class ScreenConfiguration
    {
        public float Scale => 1f;
        public uint Height { get; set; } = 1080;
        public uint Width { get; set; } = 1920;

        public bool AllowCameraMovement { get; set; } = true;
        public Key PanLeft { get; set; } = Key.A;
        public Key PanRight { get; set; } = Key.D;
        public Key PanUp { get; set; } = Key.W;
        public Key PanDown { get; set; } = Key.S;
        public Key ZoomIn { get; set; } = Key.Z;
        public Key ZoomOut { get; set; } = Key.X;
        public Key RotateRight { get; set; } = Key.Num1;
        public Key RotateLeft { get; set; } = Key.Num2;

        public FloatRect SinglePlayer { get; set; } = new FloatRect(0, 0, 1, 1);
        public FloatRect TwoPlayerLeft { get; set; } = new FloatRect(0, 0, 0.5f, 1);
        public FloatRect TwoPlayerRight { get; set; } = new FloatRect(0.5f, 0, 0.5f, 1);
        public FloatRect FourPlayerTopLeft { get; set; } = new FloatRect(0, 0, 0.5f, 0.5f);
        public FloatRect FourPlayerTopRight { get; set; } = new FloatRect(0.5f, 0, 0.5f, 0.5f);
        public FloatRect FourPlayerBottomLeft { get; set; } = new FloatRect(0, 0.5f, 0.5f, 0.5f);
        public FloatRect FourPlayerBottomRight { get; set; } = new FloatRect(0.5f, 0.5f, 0.5f, 0.5f);
    }
}
