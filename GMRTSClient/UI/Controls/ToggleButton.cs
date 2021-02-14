using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GMRTSClient.UI.Controls
{
    class ToggleButton : Button
    {
        /// <summary>
        /// Whether or not the button is currently toggled
        /// </summary>
        public bool Toggled = false;
        /// <summary>
        /// fired when the button is toggled
        /// </summary>
        public event ButtonEventHandler OnToggleOn;
        /// <summary>
        /// fired when the button is un-toggled
        /// </summary>
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