using SFML.Graphics;
using Shared.Notifications;

namespace Shared.Interfaces
{
    public interface INotificationService
    {
        public void ShowToast(ToastType type, string message);

        public void OnUpdate(float deltaT);

        public void OnRender(RenderTarget target);
    }
}
