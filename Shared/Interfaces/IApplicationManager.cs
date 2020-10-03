using Shared.Core;

namespace Shared.Interfaces
{
    public interface IApplicationManager
    {
        public void GoHome();

        public void SetActiveApplication(IApplicationInstance application);
    }
}
