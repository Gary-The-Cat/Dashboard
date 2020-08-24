using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace Shared.DataStructures
{
    public class TranslatableRectangle : RectangleShape
    {
        public List<Translation> ActiveTranslations { get; }
            = new List<Translation>();

        public TranslatableRectangle() : base()
        {
        }

        public TranslatableRectangle(Vector2f size) : base(size)
        {
        }

        public TranslatableRectangle(TranslatableRectangle copy) : base(copy)
        {
        }

        public void Update(float deltaT)
        {

        }
    }
}
