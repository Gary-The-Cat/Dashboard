using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Interfaces
{
    public interface IAmRenderable
    {
        public void OnRender(RenderTarget target);
    }
}
