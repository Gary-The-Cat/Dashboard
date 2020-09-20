using SFML.Graphics;
using Shared.Interfaces;
using System.Collections.Generic;

namespace Shared.Notifications
{
    public class NotificationService : INotificationService
    {
        private List<Toast> toastVisuals;

        public NotificationService()
        {
            toastVisuals = new List<Toast>();
        }

        public void OnRender(RenderTarget target)
        {
            toastVisuals.ForEach(t => t.OnRender(target));
            toastVisuals.RemoveAll(t => !t.IsAlive);
        }

        public void OnUpdate(float deltaT)
        {
            toastVisuals.ForEach(t => t.OnUpdate(deltaT));
        }

        public void ShowToast(ToastType type, string message)
        {
            toastVisuals.Add(new Toast(type, message));
        }
    }
}
