using SFML.Graphics;
using Shared.CameraTools;
using Shared.ScreenConfig;
using System;

namespace Shared.Core
{
    public class Screen
    {
        public Guid Id { get; set; }

        public Guid StackedParentId { get; set; }

        public bool IsStackedChild => StackedParentId != default;

        public Camera Camera { get; set; }

        public Screen(ScreenConfiguration configuration = null)
        {
            if(configuration == null)
            {
                configuration = new ScreenConfiguration();
            }

            Camera = new Camera(configuration);

            Id = Guid.NewGuid();
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

        public virtual void Start()
        {

        }
    }
}