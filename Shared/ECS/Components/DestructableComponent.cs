using System;

namespace Shared.ECS.Components
{
    public class DestructableComponent
    {
        public bool ToDestroy { get; set; }

        public Action PreDestroy { get; set; }
    }
}
