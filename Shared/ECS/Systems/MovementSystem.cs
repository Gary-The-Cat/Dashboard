using Leopotam.Ecs;
using Shared.ECS.Components;

namespace Shared.ECS.Systems
{
    [EcsInject]
    public class MovementSystem : IEcsRunSystem
    {
        private EcsWorld world = null;

        private EcsFilter<PositionComponent, MovementComponent> filter = null;

        public void Run()
        {
            foreach (var component in filter)
            {
                var mc = filter.Components2[component];
                var pc = filter.Components1[component];

                // :TODO: Refactor this and find a nicer way to pass the time through to each component using the ECS
                // Previously this was done with a static time class that was updated each frame (ew)
                pc.Position += mc.Velocity * 0.0166667f;
            }
        }
    }
}
