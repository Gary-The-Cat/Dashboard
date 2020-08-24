using Shared.CollisionData.CollisionShapes;

namespace Shared.ECS.Components
{
    public class CollisionComponent
    {
        public IShape Body;

        public float Cooldown = 0;

        public float LifeTime = 0;
    }
}
