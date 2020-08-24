using SFML.Window;
using Shared.ScreenConfig;

namespace Shared.Interfaces
{
    public interface IApplication
    {
        public Window Window { get; }

        public ScreenConfiguration Configuration { get; set; }

        public SFML.Graphics.View GetDefaultView();
    }
}
