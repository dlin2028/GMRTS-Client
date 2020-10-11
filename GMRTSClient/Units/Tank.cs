using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using GMRTSClasses.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GMRTSClient.Units
{
    class Tank : Unit, ISelectable
    {
        public bool Selected
        {
            get; set;
        }
        public Rectangle SelectionRect { get { return new Rectangle(position.ToPoint(), new Point((int)(texture.Width*scale), (int)(texture.Height*scale))); } }
        

        private Vector2 position;
        private float rotation;
        private Texture2D texture;
        private Texture2D selectionTexture;
        private float scale;
        public Tank(Vector2 position, float rotation, float scale, Texture2D texture, Texture2D selectionTexture)
        {
            this.scale = scale;
            this.position = position;
            this.rotation = rotation;
            this.texture = texture;
            this.selectionTexture = selectionTexture; 

            
        } 
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, null, Color.White, rotation, Vector2.Zero, scale, SpriteEffects.None, 0.1f);
           
            if (Selected)
                sb.Draw(selectionTexture, SelectionRect, null, Color.White, rotation, Vector2.Zero, SpriteEffects.None, 0f); 
        }
        public void DrawSelectionRect(SpriteBatch sb, Texture2D pixel)
        {
            sb.Draw(selectionTexture, SelectionRect, Color.White);
        }
    }
}
