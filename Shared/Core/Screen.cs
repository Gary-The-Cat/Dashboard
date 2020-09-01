using SFML.Graphics;
using Shared.CameraTools;
using Shared.Interfaces;
using Shared.ScreenConfig;

namespace Shared.Core
{
    public class Screen
    {
        public Camera Camera { get; set; }

        public bool IsUpdate { get; set; }

        public bool IsDraw { get; set; }

        public Screen(IApplication application)
        {
            Camera = new Camera(application.Configuration);

            IsUpdate = true;
            IsDraw = true;
        }


        public virtual void OnUpdate(float deltaT)
        {
            Camera.Update(deltaT);

        }

        public virtual void OnRender(RenderTarget target)
        {

        }

        public virtual void InitializeScreen()
        {

        }

        public void SetInactive()
        {
            IsUpdate = false;
            IsDraw = false;
        }

        public void SetActive()
        {
            IsUpdate = true;
            IsDraw = true;
        }

        public void SetUpdateInactive()
        {
            IsUpdate = false;
        }

        public void SetDrawInactive()
        {
            IsDraw = false;
        }

        public void SetUpdateActive()
        {
            IsUpdate = true;
        }

        public void SetDrawActive()
        {
            IsDraw = true;
        }
    }
}