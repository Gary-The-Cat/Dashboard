using SFML.Graphics;
using SFML.System;
using Shared.ScreenConfig;

namespace RockSlinger.CrateTools
{
    public class CrateManager
    {
        public readonly int VerticalPixelOffset = 60;
        public int CrateSize = 64;
        public RectangleShape AvailableScreen;

        private Crate[,] crates;

        public CrateManager(ScreenConfiguration config)
        {
            var x = (int)(config.Width / CrateSize);
            var y = (int)((config.Height - VerticalPixelOffset) / CrateSize);
            crates = new Crate[x, y];
            AvailableScreen = new RectangleShape(new Vector2f(config.Width, config.Height - 100));
        }

        public bool IsCrateValid(Vector2f position)
        {
            return AvailableScreen.GetGlobalBounds().Contains(position.X, position.Y);
        }

        public Vector2i GetCrateIndexFromPosition(Vector2f position)
        {
            var x = position.X;
            var y = position.Y - VerticalPixelOffset;

            return new Vector2i((int)(x / CrateSize), (int)(y / CrateSize));
        }

        public Vector2f GetCrateCentreFromIndex(Vector2i index)
        {
            var x = index.X * CrateSize + CrateSize / 2;
            var y = VerticalPixelOffset + index.Y * CrateSize + CrateSize / 2;

            return new Vector2f(x, y);
        }

        public Crate[,] GetCrates()
        {
            return crates;
        }

        public void AddCrate(Vector2f cratePosition)
        {
            if (!this.IsCrateValid(cratePosition))
            {
                return;
            }

            var crateIndex = GetCrateIndexFromPosition(cratePosition);

            if (crates[crateIndex.X, crateIndex.Y] == null)
            {
                crates[crateIndex.X, crateIndex.Y] = new Crate() { CrateType = CrateType.Full, IsCrate = true, Centroid = GetCrateCentreFromIndex(crateIndex) };
            }
            else
            {
                crates[crateIndex.X, crateIndex.Y].CrateType++;
                if ((int)crates[crateIndex.X, crateIndex.Y].CrateType > 2)
                {
                    crates[crateIndex.X, crateIndex.Y].CrateType = CrateType.Full;
                }
            }
        }
    }
}
