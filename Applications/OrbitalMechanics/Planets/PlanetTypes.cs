using System;

namespace OrbitalMechanics.Planets
{
    public enum PlanetType
    {
        Dwarf,
        Terrestrial,
        GasGiant
    }

    public static class PlanetConfigurations
    {
        public static (float, float) GetPlanetRadiusRange(PlanetType planetType)
        {
            if (planetType == PlanetType.Dwarf)
            {
                return (25, 50);
            }

            if (planetType == PlanetType.Terrestrial)
            {
                return (50, 130);
            }

            if (planetType == PlanetType.GasGiant)
            {
                return (130, 360);
            }

            return (-1, -1);
        }

        public static float GetPlanetRadius(PlanetType planetType)
        {
            var random = new Random();
            var radiusRange = GetPlanetRadiusRange(planetType);

            return radiusRange.Item1 + random.Next((int)(radiusRange.Item2 - radiusRange.Item1));
        }

        public static (float, float) GetPlanetWieghtRange(PlanetType planetType)
        {
            if (planetType == PlanetType.Dwarf)
            {
                return (1000, 40000);
            }

            if (planetType == PlanetType.Terrestrial)
            {
                return (40000, 630000);
            }

            if (planetType == PlanetType.GasGiant)
            {
                return (630000, 1200000);
            }

            return (-1, -1);
        }

        public static float GetPlanetMass(PlanetType planetType)
        {
            var random = new Random();
            var weightRange = GetPlanetWieghtRange(planetType);

            return weightRange.Item1 + random.Next((int)(weightRange.Item2 - weightRange.Item1));
        }
    }
}
