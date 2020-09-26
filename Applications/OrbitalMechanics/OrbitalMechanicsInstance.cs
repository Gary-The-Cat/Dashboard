﻿using OrbitalMechanics.Screens;
using SFML.Graphics;
using Shared.Core;
using Shared.Interfaces;

namespace OrbitalMechanics
{
    public class OrbitalMechanicsInstance : ApplicationInstanceBase, IApplicationInstance
    {
        public OrbitalMechanicsInstance(IApplication application)
        {
            this.Application = application;

            Texture texture = new Texture(new Image("Resources\\OrbitalMechanics.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new SFML.System.Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public IApplication Application { get; set; }

        public string DisplayName => "Orbital Mechanics";

        public RectangleShape Thumbnail { get; set; }

        public void AddScreen(Screen screen) => ScreenManager.AddScreen(screen);

        public void RemoveScreen(Screen screen) => ScreenManager.RemoveScreen(screen);

        public new void Initialize()
        {
            AddScreen(new OrbitalMechanicsScreen(Application, this));

            base.Initialize();
        }

        public new void Start()
        {
            base.Start();

            Application.Window.SetMouseCursorVisible(true);
        }
    }
}
