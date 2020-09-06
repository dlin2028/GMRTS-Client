using System;
using System.Collections.Generic;
using System.Text;
using GMRTSClasses.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GMRTSClient.Unit
{
    class Tank : GMRTSClasses.Units.Tank, ISelectable
    {
        public bool Selected = false;

        private Vector2 position;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        private float v;
        private Texture2D texture2D1;
        private Texture2D texture2D2;
        public Tank(Vector2 position, float rotation, Texture2D texture, Texture2D selectionTexture)
        {
            this.position = position;
            this.v = rotation;
            this.texture2D1 = texture;
            this.texture2D2 = selectionTexture;
        }

        public void Deselect()
        {
            Selected = false;
        }

        public void Select()
        {
            Selected = true;
        }
    }
}
