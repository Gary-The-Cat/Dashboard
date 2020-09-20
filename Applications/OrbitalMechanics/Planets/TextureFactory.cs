﻿using SFML.Graphics;
using SFML.System;
using Shared.ExtensionMethods;
using Shared.Maths.Noise;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrbitalMechanics.Planets
{
    public static class TextureFactory
    {
        public static Texture GetPlanetTexture(PlanetType planetType, float radius)
        {
            return new Texture(new Image(GetPixels(planetType, radius)));
        }

        private static int HeightScale = 50;

        private static double LandHeight = 0.4;

        private static double SnowHeight = 0.65;

        private static Color[,] GetPixels(PlanetType planetType, float planetRadius)
        {
            int pixels = (int)planetRadius;
            var texture = new Color[pixels * 2, pixels * 2];
            var centroid = new Vector2f(pixels, pixels);
            var baseColor = GetBaseColor(planetType);
            var secondaryColor = GetSecondaryColor(planetType, baseColor);
            bool hasRings = GetHasRings(planetType);
            bool hasStripe = GetHasStripe(planetType);
            bool hasIslands = GetHasIslands(planetType);
            var divisor = 1.0 / (pixels * 2);
            var random = new Random();

            var perlinNoise = new PerlinNoise(random);

            for (int x = 0; x < pixels * 2; x++)
            {
                for (int y = 0; y < pixels * 2; y++)
                {
                    var pixelPosition = new Vector2f(x, y);
                    var distance = centroid.Magnitude(pixelPosition);
                    if (distance <= pixels)
                    {
                        double pixelHeight = perlinNoise.GetValue(x, y, divisor);

                        var isSecondary = Math.Min(1, Math.Max(0, pixelHeight)) > LandHeight;
                        var isSnow = pixelHeight > SnowHeight;

                        Color color;

                        if (isSecondary && !isSnow)
                        {
                            color = hasIslands ? secondaryColor : baseColor;
                        }
                        else if (isSnow)
                        {
                            color = Color.White;
                        }
                        else
                        {
                            color = baseColor;
                        }

                        if (!isSnow)
                        {
                            color = color.Darken(Math.Abs(pixelHeight - LandHeight) * HeightScale);
                        }


                        texture[x, y] = color;
                    }
                }
            }

            return texture;
        }

        private static bool GetHasIslands(PlanetType planetType)
        {
            var random = new Random();
            return planetType == PlanetType.Terrestrial && random.NextDouble() > 0.2;
        }

        private static bool GetHasStripe(PlanetType planetType)
        {
            var random = new Random();
            return planetType == PlanetType.GasGiant && random.NextDouble() > 0.5;
        }

        private static bool GetHasRings(PlanetType planetType)
        {
            return false;
        }

        private static Color GetSecondaryColor(PlanetType planetType, Color baseColor)
        {
            return new Color(0x27, 0x98, 0x38);
        }

        private static Color[] DwarfColors = new Color[]
        {
            new Color(0x76, 0x54, 0x35)
        };

        private static Color[] Terrestrial = new Color[]
        {
            new Color(0x3C, 0x6B, 0x88)
        };

        private static Color[] GasGiantColors = new Color[]
        {
            new Color(0x76, 0x54, 0x35),
            new Color(0x95, 0x83, 0x67)
        };


        private static Color GetBaseColor(PlanetType planetType)
        {
            var random = new Random();

            if (planetType == PlanetType.Dwarf)
            {
                return DwarfColors[random.Next(0, DwarfColors.Length - 1)];
            }
            else if (planetType == PlanetType.Terrestrial)
            {
                return Terrestrial[0];
            }
            else if (planetType == PlanetType.GasGiant)
            {
                return GasGiantColors[random.Next(0, GasGiantColors.Length - 1)];
            }

            return Color.White;
        }
    }
}
