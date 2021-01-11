using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient
{
    abstract class UIElement
    {
        protected Texture2D texture;
        protected Color color;
        protected Rectangle rect;

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Rectangle Rect
        {
            get { return rect; }
            set { rect = value; }
        }

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public bool Enabled { get; set; }

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
        {
            Enabled = true;
            this.texture = texture;
            this.rect = rect;
            this.color = color;
        }
        public void Update()
        {
            if(Enabled)
            {
                update();
            }
        }
        protected abstract void update();

        public void Draw(SpriteBatch sb)
        {
            if(Enabled)
            {
                draw(sb);
            }
        }
        protected virtual void draw(SpriteBatch sb)
        {
            sb.Draw(texture, rect, Color);
        }
    }
}
