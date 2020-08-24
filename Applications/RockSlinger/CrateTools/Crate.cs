using SFML.System;

namespace RockSlinger.CrateTools
{
    public class Crate
    {
        public CrateType CrateType { get; set; }

        public bool IsCrate { get; set; }

        public Vector2f Centroid { get; set; }

    }

    public enum CrateType
    {
        Full,
        Broken,
        Tatters
    }
}
