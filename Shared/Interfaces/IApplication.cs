using SFML.Window;
using Shared.Core;
using Shared.ScreenConfig;

namespace Shared.Interfaces
{
    public interface IApplication
    {
        public Window Window { get; }

        public ScreenConfiguration Configuration { get; set; }

        public IApplicationManager ApplicationManager { get; }

        public SFML.Graphics.View GetDefaultView();
    }
}
