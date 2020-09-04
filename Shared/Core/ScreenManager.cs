using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Core
{
    public class ScreenManager
    {
        private Dictionary<Guid, List<Screen>> applicationScreens;

        private List<Screen> activeApplicationScreens => applicationScreens[ActiveApplication];

        public Guid ActiveApplication { get; set; }

        public ScreenManager()
        {
            applicationScreens = new Dictionary<Guid, List<Screen>>();
        }

        public void AddScreen(Screen screen)
        {
            if (!applicationScreens.ContainsKey(ActiveApplication))
            {
                applicationScreens.Add(ActiveApplication, new List<Screen>());
            }

            var screens = activeApplicationScreens;

            screens.Insert(0, screen);
        }

        public void RemoveScreen(Screen screen)
        {
            activeApplicationScreens.Remove(screen);
        }

        public void OnResize(float width, float height)
        {
            foreach (var screen in applicationScreens)
            {
                ////screen.Camera.ScaleToWindow(width, height);
            }
        }

        public void OnUpdate(float deltaT)
        {
            foreach (var screen in activeApplicationScreens.Where(s => s.IsUpdate))
            {
                screen.OnUpdate(deltaT);
            }
        }

        public void OnRender(RenderTarget target)
        {
            foreach (var screen in activeApplicationScreens.Where(s => s.IsDraw))
            {
                target.SetView(screen.Camera.GetView());
                screen.OnRender(target);
            }
        }

        public void Suspend()
        {
            foreach (var screen in activeApplicationScreens)
            {
                screen.Suspend();
            }
        }

        public void Resume()
        {
            foreach (var screen in activeApplicationScreens)
            {
                screen.Resume();
            }
        }

        public bool IsScreenActive(Screen screen)
        {
            if (!activeApplicationScreens.Contains(screen))
            {
                return false;
            }

            return screen.IsUpdate || screen.IsDraw;
        }
    }
}
