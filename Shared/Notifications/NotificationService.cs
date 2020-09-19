using SFML.Graphics;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Notifications
{
    public class NotificationService : INotificationService
    {
        public void OnRender(RenderTarget target)
        {
            throw new NotImplementedException();
        }

        public void OnUpdate(float deltaT)
        {
            throw new NotImplementedException();
        }

        public void ShowToast(ToastType type, string message)
        {
            throw new NotImplementedException();
        }
    }
}
