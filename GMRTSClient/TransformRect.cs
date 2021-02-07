using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMRTSClient
{
    class TransformRect
    {
        public Transform Transform { get; set; }
        public Vector2 Size;

        public TransformRect(Transform transform, Vector2 size)
        {
            Transform = transform;
            Size = size;
        }

        public bool Intersecting(Rectangle rect)
        {
            //may the separating axis theorem gods be with me

            var size = new Vector2(Size.X, Size.Y) * Transform.LocalScale;
            var rotationMatrix = Matrix.CreateRotationZ(Transform.LocalRotation);

            Vector2 topLeft = Vector2.Transform(Vector2.Zero, Transform.Matrix);
            Vector2 topRight = Vector2.Transform(new Vector2(Size.X, 0), Transform.Matrix);
            Vector2 bottomLeft = Vector2.Transform(new Vector2(0, Size.Y), Transform.Matrix);
            Vector2 bottomRight = Vector2.Transform(new Vector2(Size.X, Size.Y), Transform.Matrix);

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

        public bool Intersecting(Vector2 vector)
        {
            Vector2 topLeft = Vector2.Transform(Vector2.Zero, Transform.Matrix);
            Vector2 topRight = Vector2.Transform(new Vector2(Size.X, 0), Transform.Matrix);
            Vector2 bottomRight = Vector2.Transform(new Vector2(Size.X, Size.Y), Transform.Matrix);

            #region good code no need for inspection
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
    }
}
