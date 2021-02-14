using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMRTSClasses.Units;
using GMRTSClient.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GMRTSClient.Units
{
    class Unit : GMRTSClasses.Units.Unit
    {
        private Texture2D texture;

        /// <summary>
        /// The texture used for drawing
        /// </summary>
        public Texture2D Texture
        {
            get { return texture; }
            set {
                texture = value;
                Rect = new TransformRect(Transform, new Vector2(texture.Width, texture.Height));
            }
        }
        /// <summary>
        /// The texture drawn on top when selected
        /// </summary>
        public Texture2D SelectionTexture { get; set; }
        /// <summary>
        /// The position, rotation and scale of an object.
        /// </summary>
        public Transform Transform { get; set; }
        public TransformRect Rect { get; set; }
        /// <summary>
        /// The UnitActions relevant to this unit
        /// </summary>
        public LinkedList<UnitAction> Orders { get; set; }
        /// <summary>
        /// Whether or not the unit can be selected
        /// </summary>
        public bool Selectable { get; set; }
        /// <summary>
        /// Whether or not the unit is currently selected
        /// </summary>
        public bool Selected { get; set; }
        /// <summary>
        /// Whether to draw and update
        /// </summary>
        public bool Enabled { get; set; }


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
            Transform.LocalOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
            Orders = new LinkedList<UnitAction>();

            //this also initializes the rect
            Texture = texture;
            SelectionTexture = selectionTexture;
        }

        public override void Update(ulong currentMilliseconds)
        {
            base.Update(currentMilliseconds);
            Transform.UpdateTransform(CurrentPosition, CurrentRotation);
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
            sb.Draw(texture, new Rectangle((int)Transform.WorldPosition.X, (int)Transform.WorldPosition.Y, (int)(Transform.WorldScale.X * Texture.Width), (int)(Transform.WorldScale.Y * Texture.Height)), null, Color.White, Transform.WorldRotation, new Vector2(SelectionTexture.Width/2, SelectionTexture.Height/2), SpriteEffects.None, 0);
        }
        protected virtual void draw(SpriteBatch sb, Texture2D texture)
        {
            sb.Draw(texture, Transform.WorldPosition, null, Color.White, Transform.WorldRotation, Transform.WorldOrigin, Transform.WorldScale, SpriteEffects.None, 0);
        }
    }
}
