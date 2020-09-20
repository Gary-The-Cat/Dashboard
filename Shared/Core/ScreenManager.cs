using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Core
{
    public class ScreenManager
    {
        private List<Screen> screens;
        private Dictionary<Guid, Screen> screenLookup;

        public ScreenManager()
        {
            screens = new List<Screen>();
            screenLookup = new Dictionary<Guid, Screen>();
        }

        public void AddScreen(Screen screen)
        {
            screens.Insert(0, screen);
            screenLookup.Add(screen.Id, screen);
        }

        public void RemoveScreen(Screen screen)
        {
            screens.Remove(screen);
            screenLookup.Remove(screen.Id);
        }

        public void OnResize(float width, float height)
        {
            foreach (var screen in screens)
            {
                screen.Camera.ScaleToWindow(width, height);
            }
        }

        public void OnUpdate(float deltaT)
        {
            foreach (var screen in screens.Where(s => s.IsUpdate))
            {
                screen.OnUpdate(deltaT);
            }
        }

        public void OnRender(RenderTarget target)
        {
            foreach (var screen in screens.Where(s => s.IsDraw))
            {
                target.SetView(screen.Camera.GetView());
                screen.OnRender(target);
            }
        }

        public void Suspend()
        {
            foreach (var screen in screens.Where(s => s.IsUpdate))
            {
                screen.Suspend();
            }
        }

        public void Resume()
        {
            foreach (var screen in screens.Where(s => s.IsUpdate))
            {
                screen.Resume();
            }
        }

        public void Start()
        {
            foreach (var screen in screens)
            {
                screen.Start();
            }
        }

        public bool IsScreenActive(Guid screenId)
        {
            var successful = screenLookup.TryGetValue(screenId, out var screen);

            if (!successful)
            {
                return false;
            }

            return screen.IsActive;
        }

        public IEnumerable<Guid> GetScreenIds()
        {
            return screens.Select(s => s.Id);
        }
    }
}
