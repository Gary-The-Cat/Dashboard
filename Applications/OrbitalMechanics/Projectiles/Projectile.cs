using SFML.Graphics;
using SFML.System;

namespace OrbitalMechanics.Projectiles
{
    public class Projectile
    {
        public Projectile(CircleShape projectileBody, Vector2f? velocity)
        {
            ProjectileBody = projectileBody;
            Velocity = velocity;
        }

        public CircleShape ProjectileBody { get; set; }

        public Vector2f? Velocity { get; set; }

        public float Mass { get; internal set; } = 100;
    }
}
