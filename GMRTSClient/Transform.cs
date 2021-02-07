using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient
{
    class Transform
    {
        public Matrix? Parent { get; set; }
        public Matrix Matrix { get; set; }

        public Vector2 LocalOrigin
        {
            get { return origin; }
            set { origin = value; updateTransform(); }
        }

        public float LocalScale
        {
            get { return scale; }
            set { scale = value; updateTransform(); }
        }
        public Vector2 LocalPosition
        {
            get { return position; }
            set { position = value; updateTransform(); }
        }

        public float LocalRotation
        {
            get { return rotation; }
            set { LocalRotation = value; updateTransform(); }
        }

        public List<Matrix> Children
        {
            get
            {
                if (children == null) children = new List<Matrix>();

                return children;
            }
            set { children = value; }
        }

        private Vector2 position;
        private Vector2 origin;
        private float scale;
        private float rotation;
        private List<Matrix> children;

        public Transform(float rotation = 0, float scale = 1)
            :this(Vector2.Zero, rotation, scale) {}
        public Transform(Vector2 position, float rotation = 0, float scale = 1)
            : this(Vector2.Zero, Vector2.Zero, rotation, scale) {}
        public Transform(Vector2 position, Vector2 origin, float rotation = 0, float scale = 1)
        {
            Parent = null;
            this.position = position;
            this.origin = origin;
            this.rotation = rotation;
            this.scale = scale;
            updateTransform();
        }

        protected void updateTransform()
        {
            Matrix = Matrix.CreateTranslation(new Vector3(-origin, 0))
                            * Matrix.CreateScale(scale)
                            * Matrix.CreateRotationZ(rotation)
                            * Matrix.CreateTranslation(new Vector3(LocalPosition, 0));
        }
    }
}
