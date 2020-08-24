using Leopotam.Ecs;
using SFML.System;

namespace Shared.ECS.Components
{
    public class AttachableComponent
    {
        public EcsEntity AttachedEntity { get; set; }

        public bool IsAttached { get; set; }

        public Vector2f OffsetFromEntity { get; set; }
    }
}
