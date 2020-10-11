using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GMRTSClient
{
    public enum UIButtonState
    {
        None,
        Hovering,
        Pressed,
    }

    class UIElement
    {
        public delegate void ButtonEventHandler(object sender, EventArgs e);
        public event ButtonEventHandler onClick;
        public event ButtonEventHandler onHover;
        public event ButtonEventHandler onPress;
        public event ButtonEventHandler onRelease;


        public UIButtonState ButtonState;
        public bool Hovering { get; private set; }
        private Texture2D pixel;
        public Rectangle rectangle;
        public Color blue;

        public UIElement(Texture2D pixel, Rectangle rectangle, Color blue)
        {
            this.pixel = pixel;
            this.rectangle = rectangle;
            this.blue = blue;
        }

        public void Release()
        {
            onRelease.Invoke(this, EventArgs.Empty);
        }

        public void Update()
        {
            if (rectangle.Contains(InputManager.MouseState.Position))
            {
                if(!Hovering)
                {
                    onHover?.Invoke(this, EventArgs.Empty);
                }
                Hovering = true;

                if (InputManager.MouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    ButtonState = UIButtonState.Pressed;
                    if (Hovering)
                    {
                        onPress?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        Hovering = true;
                    }
                }
                else if (InputManager.LastMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    onClick?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    Hovering = false;
                    ButtonState = UIButtonState.Hovering;
                }
            }
            else
            {
                Hovering = false;
            }
        }


        public void Draw(SpriteBatch sb)
        {
            sb.Draw(pixel, rectangle, blue);
        }
    }
}