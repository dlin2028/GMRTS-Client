using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMRTSClasses.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GMRTSClient.Units
{
    class Unit : GMRTSClasses.Units.Unit
    {
        public bool Enabled { get; set; }
        public Texture2D Texture
        {
            get { return texture; }
            set {
                texture = value;
                Rect = new TransformRect(Transform, new Vector2(texture.Width, texture.Height));
            }
        }

        public Texture2D SelectionTexture { get; set; }
        public Transform Transform { get; set; }
        public TransformRect Rect { get; set; }
        public LinkedList<UnitAction> Orders { get; set; }
        public bool Selectable { get; set; }
        public bool Selected { get; set; }


        private Texture2D texture;

        public float CurrentRotation
        {
            get { return Rotation.Value; }
        }
        public Vector2 CurrentPosition
        {
            get { return new Vector2(Position.Value.X, Position.Value.Y); }
        }
         
        public Unit(Guid id, Texture2D texture, Texture2D selectionTexture)
            :base(id)
        {
            Enabled = true;
            Selectable = true;
            Selected = false;

            Transform = new Transform(CurrentPosition);
            Orders = new LinkedList<UnitAction>();

            //this also initializes the rect
            Texture = texture;
            SelectionTexture = selectionTexture;
        }

        public override void Update(ulong currentMilliseconds)
        {
            Transform.UpdateTransform(CurrentPosition, Vector2.Zero, Vector2.One, CurrentRotation);

            base.Update(currentMilliseconds);
        }

        public void Draw(SpriteBatch sb)
        {
            if (Enabled)
            {
                draw(sb, Texture);
                
                if(Selected)
                {
                    drawSelection(sb, SelectionTexture);
                }
            }
        }
        protected virtual void drawSelection(SpriteBatch sb, Texture2D texture)
        {
            Vector2 worldPosition = Transform.WorldPosition;
            Vector2 worldScale = Transform.WorldScale;
            //sb.Draw(texture, new Rectangle((int)worldPosition.X, (int)worldPosition.Y, (int)(worldScale.X * Texture.Width), (int)(Transform.LocalScale.Y * Texture.Height)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0);
            sb.Draw(texture, new Rectangle((int)worldPosition.X, (int)worldPosition.Y, (int)(worldScale.X * Texture.Width), (int)(Transform.LocalScale.Y * Texture.Height)), null, Color.White, Transform.WorldRotation, Transform.WorldOrigin, SpriteEffects.None, 0);
        }
        protected virtual void draw(SpriteBatch sb, Texture2D texture)
        {
            sb.Draw(texture, Transform.WorldPosition, null, Color.White, Transform.WorldRotation, Transform.WorldOrigin, Transform.WorldScale, SpriteEffects.None, 0);
        }
    }
}
