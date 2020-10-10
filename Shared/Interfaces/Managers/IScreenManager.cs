using Shared.Core;

namespace Shared.Interfaces.Managers
{
    public interface IScreenManager
    {
        public void AddChildScreen(Screen screen);

        public void SetActiveScreen(Screen screen);
    }
}
