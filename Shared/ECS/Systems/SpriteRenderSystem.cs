using Leopotam.Ecs;
using SFML.Graphics;
using Shared.ECS.Components;

namespace Shared.ECS.Systems
{
    [EcsInject]
    class SpriteRenderSystem : IRenderSystem
    {
        private EcsWorld world = null;

        private EcsFilter<SpriteComponent, PositionComponent> filter = null;

        public RenderWindow Window { get; set; }

        public SpriteRenderSystem()
        {
        }

        public void Run()
        {
            foreach (var component in filter)
            {
                var spriteComponent = filter.Components1[component];
                var positionComponent = filter.Components2[component];

                spriteComponent.Sprite.Position = positionComponent.Position;

                Window.Draw(spriteComponent.Sprite);
            }
        }
    }
}
