using SFML.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Core
{
    public class ScreenManager
    {
        private List<Screen> screens;

        public ScreenManager()
        {
            screens = new List<Screen>();
        }

        public void AddScreen(Screen screen)
        {
            screens.Insert(0, screen);
        }

        public void RemoveScreen(Screen screen)
        {
            screens.Remove(screen);
        }

        public void OnResize(float width, float height)
        {
            foreach (var screen in screens)
            {
                screen.Camera.ScaleToWindow(width, height);
            }
        }

        public void Update(float deltaT)
        {
            foreach (var screen in screens.Where(s => s.IsUpdate))
            {
                screen.OnUpdate(deltaT);
            }
        }

        private int count = 0;

        public void OnRender(RenderTarget target)
        {
            foreach (var screen in screens.Where(s => s.IsDraw))
            {
                target.SetView(screen.Camera.GetView());
                screen.OnRender(target);
            }
        }
    }
}
