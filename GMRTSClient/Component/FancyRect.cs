using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMRTSClient.Component
{
    class FancyRect
    {
        public bool Enabled { get; set; }

        private Transform2 transform;
        private Vector2 Size;
        public FancyRect(Transform2 transform, Size2 size)
        {
            this.transform = transform;
            this.Size = size;
            Enabled = true;
        }
        /// <summary>
        /// Returns whether or not a normal rect is intersecting
        /// </summary>
        /// <param name="rect">The other rect</param>
        /// <returns>whether or not a normal rect is intersecting</returns>
        public bool Intersecting(Rectangle rect)
        {
            if (!Enabled) return false;

            //may the separating axis theorem gods be with me

            var size = new Vector2(Size.X, Size.Y) * transform.WorldScale;
            var rotationMatrix = Matrix.CreateRotationZ(transform.WorldRotation);

            var center = Size / 2;
            Vector2 topLeft = Vector2.Transform(-center, transform.WorldMatrix);
            Vector2 topRight = Vector2.Transform(-center + new Vector2(Size.X, 0), transform.WorldMatrix);
            Vector2 bottomLeft = Vector2.Transform(-center + new Vector2(0, Size.Y), transform.WorldMatrix);
            Vector2 bottomRight = Vector2.Transform(-center + new Vector2(Size.X, Size.Y), transform.WorldMatrix);

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
        /// <summary>
        /// Returns whether or not a point is contained inside the box
        /// </summary>
        /// <param name="vector">The vector to check</param>
        /// <returns>whether or not a point is contained inside the box</returns>
        public bool Contains(Vector2 vector)
        {
            if (!Enabled) return false;

            var size = new Vector2(Size.X, Size.Y) * transform.WorldScale;
            var center = Size / 2;
            Vector2 topLeft = Vector2.Transform(-center, transform.WorldMatrix);
            Vector2 topRight = Vector2.Transform(-center + new Vector2(Size.X, 0), transform.WorldMatrix);
            Vector2 bottomRight = Vector2.Transform(-center + new Vector2(Size.X, Size.Y), transform.WorldMatrix);

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
