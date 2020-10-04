using SFML.Window;
using Shared.Core;
using Shared.Interfaces.Services;
using Shared.ScreenConfig;

namespace Shared.Interfaces
{
    public interface IApplication
    {
        public Window Window { get; }

        public ScreenConfiguration Configuration { get; set; }

        public IApplicationManager ApplicationManager { get; }

        public IEventService EventService { get; set; }

        public INotificationService NotificaitonService { get; set; }

        public IApplicationInstance ActiveApplication { get; }

        public SFML.Graphics.View GetDefaultView();
    }
}
