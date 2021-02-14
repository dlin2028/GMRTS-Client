using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace GMRTSClient
{
    public class Transform
    {
        private Vector2 position;
        private Vector2 origin;
        private Vector2 scale;
        private float rotation;
        private ObservableCollection<Transform> children;

        public Transform? Parent { get; set; }
        public Matrix Matrix { get; set; }
        public Matrix WorldMatrix
        {
            get
            {
                return Parent == null ? Matrix : Parent.WorldMatrix * Matrix;
            }
        }

        public Vector2 WorldPosition
        {
            get
            {
                return Parent == null ? LocalPosition : Vector2.Transform(LocalPosition, Parent.WorldMatrix);
            }
        }
        /// <summary>
        /// the origin relative to the parent
        /// </summary>
        public Vector2 WorldOrigin
        {
            get
            {
                return Parent == null ? LocalOrigin : Vector2.TransformNormal(LocalOrigin, Parent.WorldMatrix);
            }
        }

        public float WorldRotation
        {
            get
            {
                //idk how this works
                return Parent == null ? LocalRotation : (float)Math.Atan2(Parent.WorldMatrix.M12, Parent.WorldMatrix.M22) + LocalRotation;
            }
        }

        public Vector2 WorldScale
        {
            get
            {
                return Parent == null ? LocalScale : Vector2.TransformNormal(LocalScale, Parent.WorldMatrix);
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

        public ObservableCollection<Transform> Children
        {
            get
            {
                if (children == null)
                {
                    children = new ObservableCollection<Transform>();
                    children.CollectionChanged += Children_CollectionChanged;
                }

                return children;
            }
        }
        //Sets the parent when the children collection is changed
        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var newItem in e.NewItems)
            {
                ((Transform)newItem).Parent = this;
            }
            foreach (var oldItem in e.OldItems)
            {
                ((Transform)oldItem).Parent = null;
            }
        }
        /// <summary>
        /// Holds the position, rotation, origin, and scale of an object
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="rotation">The rotation in radians</param>
        public Transform(Vector2 position, float rotation = 0)
            : this(position, Vector2.Zero, Vector2.One, rotation) { }
        public Transform(Vector2 position, Vector2 origin, Vector2 scale, float rotation = 0)
        {
            Parent = null;
            this.position = position;
            this.origin = origin;
            this.rotation = rotation;
            this.scale = scale;
            updateTransform();
        }
        /// <summary>
        /// Updates the position and rotation to the specified values
        /// </summary>
        /// <param name="position">The new position</param>
        /// <param name="rotation">The new rotation</param>
        public void UpdateTransform(Vector2 position, float rotation)
        {
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
