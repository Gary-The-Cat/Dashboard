using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Menus
{
    public interface IAmDrawable
    {
        public Drawable GetDrawable();

        public void SetDrawableSize(Vector2f size);

        public void SetDrawableOrigin(Vector2f size);

        public void SetDrawablePosition(Vector2f size);
    }
}
