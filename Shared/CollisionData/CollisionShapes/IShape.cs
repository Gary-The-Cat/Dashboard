using System;
using System.Collections.Generic;
using SFML.System;
using System.Text;

namespace Shared.CollisionData.CollisionShapes
{
    public interface IShape
    {
        void SetPosition(float x, float y);

        Vector2f GetPosition();

        void SetPosition(Vector2f position);
    }
}
