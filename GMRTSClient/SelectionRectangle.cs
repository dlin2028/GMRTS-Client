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
        private Point selectionBegin;
        private Rectangle selectionRect;
        private Texture2D pixel;

        public SelectionRectangle(Camera camera)
        {
            Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        private Rectangle createRectangle(Point a, Point b)
        {
            return new Rectangle(Math.Min(a.X, b.X),
               Math.Min(a.Y, b.Y),
               Math.Max(Math.Abs(a.X - b.X), 1),
               Math.Max(Math.Abs(a.Y - b.Y), 1));
        }

        public void Update(ISelectable[] selectableUnits)
        {
            if(InputManager.MouseState.LeftButton == ButtonState.Pressed)
            {
                if (InputManager.LastMouseState.LeftButton != ButtonState.Pressed)
                {
                    selectionBegin = InputManager.MouseState.Position;
                }
                selectionRect = createRectangle(selectionBegin, InputManager.MouseState.Position);
            }
            else if (InputManager.MouseState.LeftButton != ButtonState.Pressed && InputManager.LastMouseState.LeftButton == ButtonState.Pressed)
            {
                foreach (var unit in selectableUnits)
                {
                    if (selectionRect.Contains(unit.SelectionRect))
                    {

                    }
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if(InputManager.MouseState.LeftButton == ButtonState.Pressed)
            {
                sb.Draw(pixel, new Rectangle(10, 20, 80, 30),Color.Green);
            }
        }
    }
}
