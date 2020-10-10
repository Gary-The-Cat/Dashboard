using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.ScreenConfig;

namespace Shared.Interfaces
{
    public interface IApplicationManager
    {
        public void GoHome();

        public void SetActiveApplication(IApplicationInstance application);

        public IApplicationInstance HomeApplication { get; }

        public IApplicationInstance ActiveApplication { get; }

        public void OnUpdate(float deltaT);

        public void OnRender(RenderTarget target);

        public Vector2u GetWindowSize();

        public View GetDefaultView();

        public ScreenConfiguration GetScreenConfiguration();

        public void SetHomeApplication(IApplicationInstance homeApplication);

        public void AddChildScreen(Screen screen);

        public void SetActiveScreen(Screen screen);
    }
}
