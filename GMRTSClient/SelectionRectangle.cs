using GMRTSClient.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient
{
    class SelectionRectangle
    {
        public List<ISelectable> SelectedUnits;

        private Point selectionBegin;
        private Rectangle selectionRect;

        private Camera camera;
        private Texture2D pixel;

        public SelectionRectangle(Camera camera, Texture2D pixel)
        {
            this.camera = camera;
            this.pixel = pixel;
        }

        private Rectangle createRectangle(Point a, Point b)
        {
            return new Rectangle(Math.Min(a.X, b.X),
               Math.Min(a.Y, b.Y),
               Math.Max(Math.Abs(a.X - b.X), 1),
               Math.Max(Math.Abs(a.Y - b.Y), 1));
        }

        public void Update(ISelectable[] selectableUnits, UIElement[] elements)
        {
            if(InputManager.MouseState.LeftButton == ButtonState.Pressed)
            {
                if(InputManager.LastMouseState.LeftButton == ButtonState.Released)
                    selectionBegin = camera.ScreenToWorldSpace(InputManager.MouseState.Position.ToVector2()).ToPoint();

                foreach (var element in elements)
                {
                    if (element.rectangle.Contains(camera.WorldToScreenSpace(selectionBegin.ToVector2())))
                    {
                        selectionRect = new Rectangle(0, 0, 0, 0);
                        return;
                    }
                }

                selectionRect = createRectangle(selectionBegin, camera.ScreenToWorldSpace(InputManager.MouseState.Position.ToVector2()).ToPoint());
            }

            if(InputManager.MouseState.RightButton == ButtonState.Released && InputManager.LastMouseState.RightButton == ButtonState.Pressed && !InputManager.Keys.IsKeyDown(Keys.LeftShift))
            {
                foreach (var unit in selectableUnits)
                {
                    unit.Selected = false;
                }
            }

            if (InputManager.MouseState.LeftButton == ButtonState.Released && InputManager.LastMouseState.LeftButton == ButtonState.Pressed)
            {
                foreach (var element in elements)
                {
                    if (element.rectangle.Contains(camera.WorldToScreenSpace(selectionBegin.ToVector2())))
                    {
                        selectionRect = new Rectangle(0, 0, 0, 0);
                        return;
                    }
                }

                SelectedUnits = new List<ISelectable>();
                foreach (var unit in selectableUnits)
                {
                    if(!InputManager.Keys.IsKeyDown(Keys.LeftShift))
                    {
                        unit.Selected = false;
                    }

                    if (selectionRect.Intersects(unit.SelectionRect))
                    {
                        if (InputManager.Keys.IsKeyDown(Keys.LeftShift))
                        {
                            unit.Selected = !unit.Selected;
                        }
                        else
                        {
                            unit.Selected = true;
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
