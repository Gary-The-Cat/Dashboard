using SFML.Graphics;

namespace Shared.Core
{
    public class Screen
    {
        public virtual void OnEnter()
        {
        }

        public virtual void OnUpdate(float dt)
        {
        }

        public virtual void OnRender(RenderTarget target)
        {
        }

        public virtual void OnExit()
        {
        }
    }
}