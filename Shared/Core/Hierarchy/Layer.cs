using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Core.Hierarchy
{
    public class Layer
    {
        public int Id { get; set; }

        public Screen ActiveScreen { get; private set; }

        public Layer(int id)
        {
            this.Id = id;
            this.screens = new List<Screen>();
        }

        public void AddScreen(Screen screen)
        {
            screens.Add(screen);
        }

        public void RemoveScreen(Screen screen)
        {
            throw new NotImplementedException();
        }

        public void SetActiveScreen(Screen screen)
        {
            if (!screens.Contains(screen))
            {
                throw new Exception("That screen cannot be found.");
            }

            ActiveScreen = screen;
        }

        private List<Screen> screens;

    }
}
