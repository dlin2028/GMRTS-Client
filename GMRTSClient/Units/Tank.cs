using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using GMRTSClasses.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GMRTSClient.Units
{
    class Tank : Unit
    {
        private float rotation;
        private Texture2D texture;
        private Texture2D selectionTexture;
        private float scale;
        public Tank(Guid id, Vector2 position, float rotation, float scale, Texture2D texture, Texture2D selectionTexture)
            :base(id)
        {
            this.scale = scale;
            CurrentPosition = position;
            this.rotation = rotation;
            this.texture = texture;
            this.selectionTexture = selectionTexture; 
        }

        public override Rectangle GetSelectionRect()
        {
            return new Rectangle(CurrentPosition.ToPoint(), new Point((int)(texture.Width * scale), (int)(texture.Height * scale)));
        }
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, CurrentPosition, null, Color.White, rotation, Vector2.Zero, scale, SpriteEffects.None, 0.1f);
           
            if (Selected)
                sb.Draw(selectionTexture, GetSelectionRect(), null, Color.White, rotation, Vector2.Zero, SpriteEffects.None, 0f); 
        }
        public void DrawSelectionRect(SpriteBatch sb, Texture2D pixel)
        {
            sb.Draw(selectionTexture, GetSelectionRect(), Color.White);
        }
    }
}
