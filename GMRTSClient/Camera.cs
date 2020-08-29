using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient
{
    class Camera
    {
        private Vector2 position;
        private float rotation;
        private float zoom;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public float Zoom
        {
            get { return zoom; }
            set { zoom = Math.Max(value, float.Epsilon); }
        }


        public Camera()
        {
            position = Vector2.One;
            zoom = 1;
        }
        public void ZoomTowardsPoint(Viewport viewport, Vector2 point, float deltaZoom)
        {
            var newZoom = Zoom + deltaZoom;
            if (newZoom <= float.Epsilon)
                return;

            var cameraCenter = ScreenToWorldSpace(viewport, new Vector2(viewport.Width/2, viewport.Height/2));

            var width = point.X - cameraCenter.X;
            var height = point.Y - cameraCenter.Y;
            position.X += width * (1 - Zoom / newZoom);
            position.Y += height * (1 - Zoom / newZoom);
            Zoom = Math.Clamp(newZoom, float.Epsilon, 2);
        }
        public Vector2 WorldToScreenSpace(Viewport viewport, Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, Transform(viewport));
        }
        public Vector2 ScreenToWorldSpace(Viewport viewport, Vector2 point)
        {
            Matrix invertedMatrix = Matrix.Invert(Transform(viewport));
            return Vector2.Transform(point, invertedMatrix);
        }
        public Matrix Transform(Viewport viewport)
        {
            int viewportWidth = viewport.Width;
            int viewportHeight = viewport.Height;

            return
                Matrix.CreateTranslation(new Vector3(-Position.X - viewportWidth / 2, -Position.Y - viewportHeight / 2, 0)) * // Translation Matrix
                Matrix.CreateRotationZ(Rotation) * // Rotation Matrix
                Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) * // Scale Matrix
                Matrix.CreateTranslation(new Vector3(viewportWidth / 2, viewportHeight / 2, 0)); // Origin/Offset Matrix
        }


    }
}
