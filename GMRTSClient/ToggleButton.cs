using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GMRTSClient
{
    class ToggleButton : Button
    {
        public bool Toggled = false;
        public event ButtonEventHandler OnToggleOn;
        public event ButtonEventHandler OnToggleOff;

        public ToggleButton(Texture2D pixel, Rectangle rect, Color color) : base(pixel, rect, color)
        {
            onClick += ToggleButton_onClick;
        }

        private void ToggleButton_onClick(object sender, System.EventArgs e)
        {
            Toggled = !Toggled;
            if(Toggled)
            {
                OnToggleOn.Invoke(this, EventArgs.Empty);
            }
            else
            {
                OnToggleOff.Invoke(this, EventArgs.Empty);
            }
        }

        public new void update()
        {
            base.update();
        }
    }
}