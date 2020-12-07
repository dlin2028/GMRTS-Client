using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMRTSClient.Units
{
    class ClientOnlyUnit : Unit
    {

        private float rotation;
        private Texture2D texture;
        private Texture2D selectionTexture;
        private float scale;
        public ClientOnlyUnit(Vector2 position, float rotation, float scale, Texture2D texture, Texture2D selectionTexture)
            : base(Guid.Empty)
        {
            Position = new GMRTSClasses.Changing<System.Numerics.Vector2>(new System.Numerics.Vector2(position.X, position.Y), System.Numerics.Vector2.Zero, null, 0);
            this.scale = scale;
            this.rotation = rotation;
            this.texture = texture;
            this.selectionTexture = selectionTexture;
        }
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, CurrentPosition, null, Color.White, rotation, new Vector2(texture.Width, texture.Height)/2f, scale, SpriteEffects.None, 0.1f);

            if (Selected)
                sb.Draw(selectionTexture, new Rectangle((int)CurrentPosition.X, (int)CurrentPosition.Y, (int)(texture.Width * scale), (int)(texture.Height * scale)),null, Color.White, rotation, new Vector2(texture.Width, texture.Height) / 2f, SpriteEffects.None, 0f);
        }

        public override bool Intersecting(Rectangle rect)
        {
            //may the separating axis theorem gods be with me

            var size = new Vector2(texture.Width, texture.Height) * scale;
            var rotationMatrix = Matrix.CreateRotationZ(rotation);

            var topLeft = CurrentPosition + Vector2.Transform(-size / 2f, rotationMatrix);
            var topRight = CurrentPosition + Vector2.Transform(new Vector2(size.X, -size.Y) / 2f, rotationMatrix);
            var bottomLeft = CurrentPosition + Vector2.Transform(new Vector2(-size.X, size.Y) / 2f, rotationMatrix);
            var bottomRight = CurrentPosition + Vector2.Transform(size / 2f, rotationMatrix);

            Vector2[] verticies = { topLeft, topRight, bottomLeft, bottomRight };
            Vector2[] rectVerticies = { new Vector2(rect.X, rect.Y), new Vector2(rect.X + rect.Width, rect.Y), new Vector2(rect.X, rect.Y + rect.Height), new Vector2(rect.X + rect.Width, rect.Y + rect.Height) };
            Vector2[] projections = { topLeft - topRight, topLeft - bottomLeft, Vector2.UnitX, Vector2.UnitY };

            foreach (var proj in projections)
            {
                var projs = verticies.Select(x => Vector2.Dot(x, proj));
                var rectProjs = rectVerticies.Select(x => Vector2.Dot(x, proj));

                if (!(rectProjs.Min() <= projs.Max() && projs.Min() <= rectProjs.Max()))
                {
                    return false;
                }
            }

            return true;
        }
        public override void Update(ulong currentMilliseconds)
        {

        }
    }
}
