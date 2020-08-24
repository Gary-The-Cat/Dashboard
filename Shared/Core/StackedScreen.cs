using SFML.Graphics;
using System;
using System.Collections.Generic;

namespace Shared.Core
{
    /// <summary>
    /// Stacked Screen - A simple Screen implementation that facilitates
    /// a stack of screens to be rendered in a known order.
    /// </summary>
    public class StackedScreen : Screen
    {
        private List<Screen> screens;

        public StackedScreen()
        {
            screens = new List<Screen>();
        }

        /// <summary>
        /// Add Screen - Adds the provided screen to the end of the list of stacked screens.
        /// </summary>
        /// <param name="screen">A reference to the screen to be added.</param>
        public void AddScreen(Screen screen)
        {
            screens.Add(screen);
        }

        /// <summary>
        /// Insert Screen - Inserts the provided screen in the provided index inside the stacked screens.
        /// </summary>
        /// <param name="screen">A reference to the screen to be inserted.</param>
        /// <param name="index">The desired index to insert.</param>
        public void InsertScreen(Screen screen, int index)
        {
            if (index < 0)
            {
                throw new Exception("Screen cannot be inserted outside of the array bounds.");
            }

            screens.Insert(index, screen);
        }

        /// <summary>
        /// Remove Screen - Removes the provided screen from the list of stacked screens
        /// </summary>
        /// <param name="screen">A reference to the screen to be removed.</param>
        /// <returns>True/False insicating if removing the screen was successful.</returns>
        public bool RemoveScreen(Screen screen)
        {
            return screens.Remove(screen);
        }

        /// <summary>
        /// Call OnEnter for each screen in the stack.
        /// </summary>
        public override void OnEnter()
        {
            screens.ForEach(s => s.OnEnter());
        }

        /// <summary>
        /// Call OnUpdate(dt) for each screen in the stack.
        /// </summary>
        public override void OnUpdate(float dt)
        {
            screens.ForEach(s => s.OnUpdate(dt));
        }

        /// <summary>
        /// Call OnRender(target) for each screen in the stack.
        /// </summary>
        /// <param name="target"></param>
        public override void OnRender(RenderTarget target)
        {
            screens.ForEach(s => s.OnRender(target));
        }

        /// <summary>
        /// Call OnExit for each screen in the stack.
        /// </summary>
        public override void OnExit()
        {
            screens.ForEach(s => s.OnExit());
        }
    }
}
