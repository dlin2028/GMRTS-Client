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
                sb.Draw(selectionTexture, new Rectangle((int)CurrentPosition.X, (int)CurrentPosition.Y, (int)(texture.Width * scale), (int)(texture.Height * scale)),null, Color.White, rotation, new Vector2(selectionTexture.Width, selectionTexture.Height) / 2f, SpriteEffects.None, 0f);
        }
        public override bool Intersecting(Vector2 vector)
        {
            var size = new Vector2(texture.Width, texture.Height) * scale;
            var rotationMatrix = Matrix.CreateRotationZ(rotation);

            var topLeft = CurrentPosition + Vector2.Transform(-size / 2f, rotationMatrix);
            var topRight = CurrentPosition + Vector2.Transform(new Vector2(size.X, -size.Y) / 2f, rotationMatrix);
            //var bottomLeft = CurrentPosition + Vector2.Transform(new Vector2(-size.X, size.Y) / 2f, rotationMatrix);
            var bottomRight = CurrentPosition + Vector2.Transform(size / 2f, rotationMatrix);

            #region bad code don't look
            var x = vector.X;
            var y = vector.Y;
            var ax = topLeft.X;
            var ay = topLeft.Y;
            var bx = topRight.X;
            var by = topRight.Y;
            var dx = bottomRight.X;
            var dy = bottomRight.Y;

            var bax = bx - ax;
            var bay = by - ay;
            var dax = dx - ax;
            var day = dy - ay;

            if ((x - ax) * bax + (y - ay) * bay < 0.0) return false;
            if ((x - bx) * bax + (y - by) * bay > 0.0) return false;
            if ((x - ax) * dax + (y - ay) * day < 0.0) return false;
            if ((x - dx) * dax + (y - dy) * day > 0.0) return false;

            // "if we connect the point to three vertexes of the rectangle then the angles between those segments and sides should be acute" - some smart guy
            // https://stackoverflow.com/a/2752754

            //yes this can be sped up using matrix math
            //no i don't want to do it

            return true;
            #endregion
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
            //override and do nothing
        }
    }
}
