using SFML.System;
using Shared.ScreenConfig;
using System;

namespace OrbitalMechanics.Planets
{
    public static class PlanetFactory
    {
        public static Planet GetPlanet(ScreenConfiguration configuration)
        {
            var planetType = PlanetType.Terrestrial;
            var radius = PlanetConfigurations.GetPlanetRadius(planetType);
            var mass = PlanetConfigurations.GetPlanetMass(planetType);
            var position = GetOnScreenPosition(radius, configuration);
            var texture = TextureFactory.GetPlanetTexture(planetType, radius);

            return new Planet(texture, position, radius, mass);
        }

        public static Vector2f GetOnScreenPosition(float radius, ScreenConfiguration configuration)
        {
            var random = new Random();

            var xRange = (int)(configuration.Width - (radius * 2));
            var yRange = (int)(configuration.Height - (radius * 2));

            var x = radius + (int)(random.NextDouble() * xRange);
            var y = radius + (int)(random.NextDouble() * yRange);

            return new Vector2f(x, y);
        }
    }
}
