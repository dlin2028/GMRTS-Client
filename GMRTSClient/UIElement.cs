using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient
{
    abstract class UIElement : Sprite
    {
        protected Texture2D texture;
        protected Color color;
        protected Rectangle rect;

        public Rectangle Rect
        {
            get { return rect; }
            set { rect = value; }
        }

        public Point Location
        {
            get
            {
                return new Point(rect.X, rect.Y);
            }
            set
            {
                rect = new Rectangle(value.X, value.Y, rect.Width, rect.Height);
            }
        }

        public UIElement(Texture2D texture, Rectangle rect, Color color)
            :base(texture, 1f)
        {
            Enabled = true;
            this.texture = texture;
            this.rect = rect;
            this.color = color;
        }
    }
}
