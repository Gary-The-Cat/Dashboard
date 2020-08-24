using Shared.Core;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfDriving.Screens
{
    public class MapMakingScreen : Screen
    {
        private IApplication application;

        public MapMakingScreen(IApplication application)
        {
            this.application = application;
        }
    }
}
