﻿using Shared.Core;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfDriving.Screens
{
    public class RaceScreen : Screen
    {
        public RaceScreen(
            IApplication application,
            IApplicationInstance applicationInstance,
            Screen parentScreen) : base(application.Configuration, applicationInstance)
        {

        }
    }
}
