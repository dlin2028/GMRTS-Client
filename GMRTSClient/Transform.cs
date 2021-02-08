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

        public Vector2 WorldPosition
        {
            get
            {
                return Parent == null ? LocalPosition : Vector2.Transform(LocalPosition, Parent.Value);
            }
        }
        public Vector2 WorldOrigin
        {
            get
            {
                return Parent == null ? LocalOrigin : Vector2.Transform(LocalOrigin, Parent.Value);
            }
        }

        public float WorldRotation
        {
            get
            {
                //idk how this works
                return Parent == null ? LocalRotation : (float)Math.Atan2(Parent.Value.M12, Parent.Value.M22) + LocalRotation;
            }
        }

        public Vector2 WorldScale
        {
            get
            {
                return Parent == null ? LocalScale : Vector2.TransformNormal(LocalScale, Parent.Value);
            }
        }

        public Vector2 LocalOrigin
        {
            get { return origin; }
            set { origin = value; updateTransform(); }
        }

        public Vector2 LocalScale
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
        private Vector2 scale;
        private float rotation;
        private List<Matrix> children;

        public Transform(Vector2 position, float rotation = 0)
            : this(position, Vector2.Zero, Vector2.One, rotation) {}
        public Transform(Vector2 position, Vector2 origin, Vector2 scale, float rotation = 0)
        {
            Parent = null;
            this.position = position;
            this.origin = origin;
            this.rotation = rotation;
            this.scale = scale;
            updateTransform();
        }

        public void UpdateTransform(Vector2 position, Vector2 origin, Vector2 scale, float rotation)
        {
            this.origin = origin;
            this.scale = scale;
            this.rotation = rotation;
            this.position = position;
            updateTransform();
        }

        protected void updateTransform()
        {
            Matrix = Matrix.CreateTranslation(new Vector3(-origin, 0))
                            * Matrix.CreateScale(new Vector3(scale, 0))
                            * Matrix.CreateRotationZ(rotation)
                            * Matrix.CreateTranslation(new Vector3(LocalPosition, 0));
        }
    }
}
