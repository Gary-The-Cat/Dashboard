using SFML.Graphics;

namespace Shared.Core
{
    public class Screen
    {
        public bool IsActive { get; set; }

        public virtual void OnEnter()
        {
            IsActive = true;
        }

        public virtual void OnUpdate(float dt)
        {
        }

        public virtual void OnRender(RenderTarget target)
        {
        }

        public virtual void OnExit()
        {
            IsActive = false;
        }
    }
}