using Leopotam.Ecs;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.ECS
{
    public interface IRenderSystem : IEcsRunSystem
    {
        RenderWindow Window { get; set; }
    }
}
