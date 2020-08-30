﻿using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.Core;
using Shared.Interfaces;
using System;
using System.Collections.Generic;

namespace Dashboard.Home
{
    public class ApplicationDashboard
    {
        public bool IsActive { get; set; }

        public ApplicationInstanceVisual SelectedApplication => applications[selectedApplicationIndex];

        private Vector2u selectedIconSize;
        private Vector2u nonSelectedIconSize;
        private List<ApplicationInstanceVisual> applications;
        private bool transitioning;
        private Vector2u size;
        private float gapSize;
        private int selectedApplicationIndex = 0;
        private Font defaultFont;
        private Sprite background;

        public ApplicationDashboard(List<ApplicationInstanceVisual> applications, IApplication application)
        {
            defaultFont = new Font("Resources\\font.ttf");
            selectedIconSize = new Vector2u(300, 300);
            nonSelectedIconSize = new Vector2u(220, 220);
            this.applications = applications;
            this.size = application.Window.Size;
            this.UpdateSizingAndSpacing();

            Texture blueprint = new Texture(CreateBlueprint(size.X, size.Y));
            background = new Sprite(blueprint);

            application.Window.KeyPressed += KeyPressed;

            IsActive = true;
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (!IsActive)
            {
                return;
            }

            // TODO replace this with Key Just Pressed code & Add ability to interact with mouse
            if(e.Code == Keyboard.Key.Left)
            {
                selectedApplicationIndex -= 1;
                if(selectedApplicationIndex < 0)
                {
                    selectedApplicationIndex = applications.Count - 1;
                }
            }
            else if (e.Code == Keyboard.Key.Right)
            {
                selectedApplicationIndex += 1;
                if (selectedApplicationIndex >= applications.Count)
                {
                    selectedApplicationIndex = 0;
                }
            }

            // Note this should start an animated transition using the tween/easings code.
        }

        public void Update(float deltaT)
        {
            if (!transitioning)
            {

            }
            else
            {
                // Update the transition
            }
        }

        public void Draw(RenderTarget target)
        {
            if (!transitioning)
            {
                var selected = GetSelectedApplicationIndex();
                var left = GetLeftApplicationIndex();
                var right = GetRightApplicationIndex();

                var selectedVisual = new RectangleShape(new Vector2f(selectedIconSize.X, selectedIconSize.Y))
                {
                    Texture = applications[selected].Image.Texture,
                    Position = GetSelectedApplicationPosition()
                };

                var selectedText = new Text(applications[selected].DisplayName, defaultFont, 28)
                {
                    Position = new Vector2f(size.X / 2, selectedVisual.Position.Y + 310),
                    FillColor = Color.White
                };

                var bounds = selectedText.GetLocalBounds();
                selectedText.Origin = new Vector2f(bounds.Width / 2, bounds.Height / 2);

                var leftVisual = new RectangleShape(new Vector2f(nonSelectedIconSize.X, nonSelectedIconSize.Y))
                {
                    Texture = applications[left].Image.Texture,
                    Position = GetLeftApplicationPosition()
                };

                var rightVisual = new RectangleShape(new Vector2f(nonSelectedIconSize.X, nonSelectedIconSize.Y))
                {
                    Texture = applications[right].Image.Texture,
                    Position = GetRightApplicationPosition()
                };

                target.Draw(background);
                target.Draw(selectedVisual);
                target.Draw(leftVisual);
                target.Draw(rightVisual);
                target.Draw(selectedText);
            }
            else
            {
                // Draw the applications positions/scales
            }
        }

        private int GetSelectedApplicationIndex()
        {
            return selectedApplicationIndex;
        }

        private int GetLeftApplicationIndex()
        {
            return selectedApplicationIndex > 0
                ? selectedApplicationIndex - 1
                : applications.Count - 1;
        }

        private int GetRightApplicationIndex()
        {
            return selectedApplicationIndex < (applications.Count - 1)
                ? selectedApplicationIndex + 1
                : 0;
        }

        private Vector2f GetSelectedApplicationPosition()
        {
            var x = size.X / 2 - selectedIconSize.X / 2;
            var y = size.Y / 2 - selectedIconSize.Y / 2;
            return new Vector2f(x, y);
        }

        private Vector2f GetLeftApplicationPosition()
        {
            // Get the position of the bottom left hand corner of the thumbnail
            var x = gapSize;
            var y = (size.Y / 2) - (nonSelectedIconSize.Y / 2);

            return new Vector2f(x, y);
        }

        private Vector2f GetRightApplicationPosition()
        {
            // Right hand side - gap - width of icon gives left hand side.
            var x = size.X - gapSize - nonSelectedIconSize.X;

            //
            var y = (size.Y / 2) - (nonSelectedIconSize.Y / 2);

            return new Vector2f(x, y);
        }

        private void UpdateSizingAndSpacing()
        {
            // Ensure that the selected application thumbnail sizes will fit on our screen with a reasonable gap

            var totalIconWidth = selectedIconSize.X + 2 * nonSelectedIconSize.X;
            var remainingSize = size.X - totalIconWidth;
            gapSize = remainingSize / 4;

            if(gapSize < 50)
            {
                selectedIconSize /= 2;
                nonSelectedIconSize /= 2;
                UpdateSizingAndSpacing();
            }
        }

        private Image CreateBlueprint(uint width, uint height)
        {
            Color background = new Color(19, 93, 168);
            Color thick = new Color(57, 121, 185);
            Color thin = new Color(36, 108, 182);

            void DrawVerticalLine(Image image, uint x, Color color)
            {
                if (x < image.Size.X)
                {
                    for (uint y = 0; y < image.Size.Y; y++)
                    {
                        image.SetPixel(x, y, color);
                    }
                }
            }

            void DrawHorizontalLine(Image image, uint y, Color color)
            {
                if (y < image.Size.Y)
                {
                    for (uint x = 0; x < image.Size.X; x++)
                    {
                        image.SetPixel(x, y, color);
                    }
                }
            }

            Image image = new Image(width, height, background);

            int rowCount = (int)Math.Ceiling(height / 100.0);
            int colCount = (int)Math.Ceiling(width / 100.0);

            int gridWidth = colCount * 100;
            int gridHeight = rowCount * 100;

            int startX = 0 - (gridWidth - (int)width) / 2;
            int startY = 0 - (gridHeight - (int)height) / 2;

            for (int col = 0; col < colCount; col++)
            {
                var x = (uint)(startX + col * 100);
                DrawVerticalLine(image, x + 0, thick);
                DrawVerticalLine(image, x + 25, thin);
                DrawVerticalLine(image, x + 50, thick);
                DrawVerticalLine(image, x + 75, thin);
                DrawVerticalLine(image, x + 99, thick);
            }

            for (int row = 0; row < rowCount; row++)
            {
                var y = (uint)(startY + row * 100);
                DrawHorizontalLine(image, y + 0, thick);
                DrawHorizontalLine(image, y + 25, thin);
                DrawHorizontalLine(image, y + 50, thick);
                DrawHorizontalLine(image, y + 75, thin);
                DrawHorizontalLine(image, y + 99, thick);
            }

            return image;
        }
    }
}