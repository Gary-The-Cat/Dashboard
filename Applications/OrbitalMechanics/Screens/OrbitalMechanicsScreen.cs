using OrbitalMechanics.Planets;
using OrbitalMechanics.Projectiles;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.Core;
using Shared.ExtensionMethods;
using Shared.Interfaces;
using Shared.ScreenConfig;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrbitalMechanics.Screens
{
    public class OrbitalMechanicsScreen : Screen
    {
        internal int frame;

        readonly Vector2f TitleBarSize = new Vector2f(12, 57);

        List<Drawable> entities;

        public RectangleShape background;

        private bool isThrowing;
        private bool isLaunching;

        private Vector2f? throwAnchor;

        private Vertex[] elastic;

        private Projectile projectile;

        private Goal goal;

        private List<Planet> planets;

        private List<Vector2f> projections;

        private const int planetCount = 3;

        private ScreenConfiguration Configuration { get; set; }

        private IApplication Application { get; set; }

        public OrbitalMechanicsScreen(
            IApplication application,
            IApplicationInstance applicationInstance) 
            : base(application.Configuration, applicationInstance)
        {
            frame = 0;

            this.Application = application;

            this.Configuration = application.Configuration;

            application.Window.KeyPressed += KeyPressed;
            application.Window.MouseButtonPressed += MousePressed;
            application.Window.MouseButtonReleased += MouseReleased;

            background = new RectangleShape(new Vector2f(Configuration.Width, Configuration.Height));
            background.Texture = new Texture(new Image("Resources\\Nightscape.png"));


            entities = new List<Drawable>();
            projectile = new Projectile(new CircleShape(30) { Origin = new Vector2f(30, 30) }, null);
            elastic = new Vertex[2];
            projections = new List<Vector2f>();

            PopulatePlanets();

            SetNewGoal();
            SetNewStart();
        }

        private void PopulatePlanets()
        {
            planets = new List<Planet>();

            for (int i = 0; i < planetCount; i++)
            {
                AddPlanet();
            }
        }

        private void AddPlanet()
        {
            var planet = PlanetFactory.GetPlanet(Configuration);
            while (planets.Any(p => p.PlanetBody.Position.Magnitude(planet.PlanetBody.Position) < (p.Radius + planet.Radius + 40)))
            {
                planet.PlanetBody.Position = PlanetFactory.GetOnScreenPosition(planet.PlanetBody.Radius, Configuration);
            }

            planets.Add(planet);
        }

        private void MouseReleased(object sender, MouseButtonEventArgs e)
        {
            isThrowing = false;
            isLaunching = true;
            LaunchProjectile();
        }

        private void LaunchProjectile()
        {
            var delta = throwAnchor - projectile.ProjectileBody.Position;
            projectile.Velocity = delta.Value;
        }

        private void MousePressed(object sender, MouseButtonEventArgs e)
        {
            isThrowing = true;
            elastic[0] = new Vertex(throwAnchor.Value, Color.White);
            projectile.Velocity = null;
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.N)
            {
                Task.Run(() =>
                {
                    PopulatePlanets();
                });
            }
        }

        /// <summary>
        /// Update - Here we add all our logic for updating components in this screen. 
        /// This includes checking for user input, updating the position of moving objects and more!
        /// </summary>
        /// <param name="deltaT">The amount of time that has passed since the last frame was drawn.</param>
        public override void OnUpdate(float deltaT)
        {
            goal.Update(deltaT);

            if (isThrowing)
            {
                projectile.ProjectileBody.Position = SetProjectilePosition();
                elastic[1] = new Vertex(projectile.ProjectileBody.Position, Color.White);
                SetProjections();
            }
            else
            {
                if (isLaunching)
                {
                    projectile.ProjectileBody.Position += projectile.Velocity.Value * deltaT;
                    if (projectile.ProjectileBody.Position.Magnitude(throwAnchor.Value) < 5)
                    {
                        isLaunching = false;
                    }
                }
                else if (projectile.Velocity != null)
                {
                    projectile.Velocity += GetNewVelocity(projectile.ProjectileBody.Position, projectile.Mass, deltaT);
                    projectile.ProjectileBody.Position += projectile.Velocity.Value * deltaT;
                }

                CheckGoal();

                if (!CheckPlanetCollisions(projectile))
                {
                    projectile.Velocity = new Vector2f(0, 0);
                    projectile.ProjectileBody.Position = new Vector2f(-50, -50);
                }
            }
        }

        private Vector2f SetProjectilePosition()
        {
            var mousePosition = GetMousePosition();
            var direction = (mousePosition - throwAnchor.Value);

            if (direction.Magnitude() > 800)
            {
                return throwAnchor.Value + direction.Normalize() * 800;
            }

            return mousePosition;
        }

        private bool CheckPlanetCollisions(Projectile projectile)
        {
            foreach (var planet in planets)
            {
                var distance = planet.PlanetBody.Position.Magnitude(projectile.ProjectileBody.Position);

                if (distance < planet.Radius + projectile.ProjectileBody.Radius)
                {
                    return false;
                }
            }

            return true;
        }

        private void SetProjections()
        {
            projections.Clear();

            var projectileClone = new Projectile(
                new CircleShape(projectile.ProjectileBody.Radius)
                {
                    Position = projectile.ProjectileBody.Position,
                    Origin = new Vector2f(30, 30)
                },
                null)
            { Velocity = projectile.Velocity };


            var delta = throwAnchor - projectile.ProjectileBody.Position;
            projectileClone.Velocity = delta.Value;
            bool launching = true;

            for (int i = 0; i < 8; i++)
            {
                float time = 0;
                float timeStep = 1 / 60f;
                while (time < 0.4)
                {
                    if (launching)
                    {
                        projectileClone.ProjectileBody.Position += projectileClone.Velocity.Value * timeStep;
                        if (projectileClone.ProjectileBody.Position.Magnitude(throwAnchor.Value) < 5)
                        {
                            launching = false;
                        }
                    }
                    else
                    {
                        var newVelocity = GetNewVelocity(projectileClone.ProjectileBody.Position, projectileClone.Mass, timeStep);
                        projectileClone.Velocity += newVelocity;
                        projectileClone.ProjectileBody.Position += projectileClone.Velocity.Value * timeStep;
                    }

                    time += timeStep;

                    if (!CheckPlanetCollisions(projectileClone))
                    {
                        return;
                    }
                }

                projections.Add(projectileClone.ProjectileBody.Position);
            }
        }

        private void CheckGoal()
        {
            if (projectile.ProjectileBody.Position.Magnitude(goal.Position) < 30)
            {
                PopulatePlanets();

                SetNewGoal();
            }
        }

        private void SetNewGoal()
        {
            projectile.Velocity = null;

            goal = new Goal(PlanetFactory.GetOnScreenPosition(30, Configuration), 30);

            while (planets.Any(p => p.PlanetBody.Position.Magnitude(goal.Position) < (p.Radius + goal.Radius + 100)))
            {
                goal = new Goal(PlanetFactory.GetOnScreenPosition(30, Configuration), 30);
            }

            SetNewStart();
        }

        private void SetNewStart()
        {
            projectile.ProjectileBody.Position = PlanetFactory.GetOnScreenPosition(projectile.ProjectileBody.Radius, Configuration);

            while (planets.Any(p => p.PlanetBody.Position.Magnitude(goal.Position) < (p.Radius + goal.Radius + 100))
                || projectile.ProjectileBody.Position.Magnitude(goal.Position) < Configuration.Width / 2)
            {
                projectile.ProjectileBody.Position = PlanetFactory.GetOnScreenPosition(projectile.ProjectileBody.Radius, Configuration);
            }

            throwAnchor = projectile.ProjectileBody.Position;
        }

        private Vector2f GetNewVelocity(Vector2f position, float mass, float deltaT)
        {
            var deltaV = new Vector2f();

            foreach (var planet in planets)
            {
                float force = (float)(0.0000000000667 * (planet.Mass * 100)) / planet.Position.Magnitude(position);

                var directionVector = (planet.Position - position).Normalize();
                deltaV += force * deltaT * directionVector * 30000000;
            }

            return deltaV;
        }

        /// <summary>
        /// Draw - Here we don't update any of the components, only draw them in their current state to the screen.
        /// </summary>
        /// <param name="deltaT">The amount of time that has passed since the last frame was drawn.</param>
        public override void OnRender(RenderTarget target)
        {
            target.Draw(background);
            foreach (var planet in planets.ToList())
            {
                target.Draw(planet.PlanetBody);
            }

            foreach (var entity in entities)
            {
                target.Draw(entity);
            }

            goal.Draw(target);

            if (projectile != null)
            {
                target.Draw(projectile.ProjectileBody);
                if (isThrowing)
                {
                    var outerSize = throwAnchor.Value.Magnitude(projectile.ProjectileBody.Position) / 8;
                    outerSize = outerSize == 0 ? 1 : outerSize;
                    var colour = new Color(255, (byte)(255 - outerSize), (byte)(255 - outerSize));

                    var anchorCentre = new CircleShape(10)
                    {
                        Position = throwAnchor.Value,
                        Origin = new Vector2f(10, 10),
                        FillColor = colour
                    };

                    var anchorOuter = new CircleShape(outerSize)
                    {
                        Position = throwAnchor.Value,
                        Origin = new Vector2f(outerSize, outerSize),
                        FillColor = Color.Transparent,
                        OutlineThickness = 1,
                        OutlineColor = colour
                    };


                    target.Draw(anchorCentre);
                    target.Draw(anchorOuter);
                }
            }

            if (isThrowing)
            {
                int position = 0;
                byte alpha;
                foreach (var projection in projections)
                {
                    var radius = 10 - position;
                    alpha = (byte)(255 - (byte)(255 * (position / 8.0)));
                    target.Draw(new CircleShape(radius)
                    {
                        Position = projection,
                        Origin = new Vector2f(radius, radius),
                        FillColor = new Color(255, 255, 255, alpha)
                    });
                    position++;
                }
            }

            frame++;
        }

        public Vector2f GetMousePosition()
        {
            var position = Mouse.GetPosition();

            var adjustedPosition = position - Application.Window.Position;

            return new Vector2f(adjustedPosition.X - TitleBarSize.X, adjustedPosition.Y - TitleBarSize.Y);
        }

        public void AddVisual(Drawable visual)
        {
            entities.Add(visual);
        }
    }
}
