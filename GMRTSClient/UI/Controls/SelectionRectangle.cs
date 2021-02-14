using GMRTSClient.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.UI.Controls
{
    /// <summary>
    /// The rectangle to select units when click+dragging
    /// </summary>
    class SelectionRectangle
    {
        /// <summary>
        /// The currently selected selectable units
        /// </summary>
        public List<Unit> SelectedUnits;

        private Point selectionBegin;
        private Rectangle selectionRect;

        private Camera camera;
        private Texture2D pixel;

        public SelectionRectangle(Camera camera, Texture2D pixel)
        {
            this.camera = camera;
            this.pixel = pixel;
        }

        /// <summary>
        /// Create rectangle given two points
        /// </summary>
        /// <param name="a">First point</param>
        /// <param name="b">Second point</param>
        /// <returns></returns>
        private Rectangle createRectangle(Point a, Point b)
        {
            return new Rectangle(Math.Min(a.X, b.X),
               Math.Min(a.Y, b.Y),
               Math.Max(Math.Abs(a.X - b.X), 1),
               Math.Max(Math.Abs(a.Y - b.Y), 1));
        }

        public void Update(Unit[] selectableUnits, UIElement[] elements)
        {
            if(InputManager.MouseState.LeftButton == ButtonState.Pressed)
            {
                if(InputManager.LastMouseState.LeftButton == ButtonState.Released)
                    selectionBegin = camera.ScreenToWorldSpace(InputManager.MouseState.Position.ToVector2()).ToPoint();

                foreach (var element in elements)
                {
                    if (element.Rect.Contains(camera.WorldToScreenSpace(selectionBegin.ToVector2())))
                    {
                        selectionRect = new Rectangle(0, 0, 0, 0);
                        return;
                    }
                }

                selectionRect = createRectangle(selectionBegin, camera.ScreenToWorldSpace(InputManager.MouseState.Position.ToVector2()).ToPoint());
            }
            else if (InputManager.LastMouseState.LeftButton == ButtonState.Pressed)
            {
                foreach (var element in elements)
                {
                    if (element.Rect.Contains(camera.WorldToScreenSpace(selectionBegin.ToVector2())))
                    {
                        selectionRect = new Rectangle(0, 0, 0, 0);
                        return;
                    }
                }

                SelectedUnits = new List<Unit>();
                foreach (var unit in selectableUnits)
                {
                    if(!InputManager.Keys.IsKeyDown(Keys.LeftShift))
                    {
                        unit.Selected = false;
                    }

                    if (unit.Rect.Intersecting(selectionRect))
                    {
                        if (InputManager.Keys.IsKeyDown(Keys.LeftShift))
                        {
                            unit.Selected = !unit.Selected;
                        }
                        else
                        {
                            unit.Selected = true;
                        }
                    }

                    if (unit.Selected)
                    {
                        SelectedUnits.Add(unit);
                    }
                    else if (SelectedUnits.Contains(unit))
                    {
                        SelectedUnits.Remove(unit);
                    }
                }
            }
        }

        public void DeselectAll()
        {
            foreach (var unit in SelectedUnits)
            {
                unit.Selected = false;
            }
            SelectedUnits.Clear();
        }

        public void Draw(SpriteBatch sb)
        {
            if(InputManager.MouseState.LeftButton == ButtonState.Pressed)
            {
                sb.Draw(pixel, selectionRect,Color.Green);
            }
        }
    }
}
