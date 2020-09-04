using Shared.Core;

namespace Shared.Interfaces
{
    public interface IApplicationManager
    {
        public void AddScreen(Screen screen);

        public bool IsScreenActive(Screen screen);
    }
}
