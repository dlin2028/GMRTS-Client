using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace GMRTSClient
{
    class Camera
    {
        private Vector2 position;
        private float rotation;
        private float zoom;
        private Viewport viewport;

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
        private float minZoom;
        private float maxZoom;

        public Camera(float minZoom = 0.005f, float maxZoom = 0.25f)
        {
            this.minZoom = minZoom;
            this.maxZoom = maxZoom;

            position = Vector2.Zero;
            zoom = (maxZoom + minZoom)/2f;
        }
        public void Pan(Vector2 distance, bool worldSpace = false)
        {
            if (worldSpace)
                position -= distance;
            else
                position -= distance * (1/Zoom );
        }
        public void ZoomTowardsPoint(Viewport viewport, Vector2 point, float deltaZoom)
        {
            var newZoom = Zoom + Zoom * deltaZoom;
            if (newZoom <= minZoom || newZoom >= maxZoom)
                return;

            var cameraCenter = ScreenToWorldSpace(new Vector2(viewport.Width/2, viewport.Height/2));

            var width = point.X - cameraCenter.X;
            var height = point.Y - cameraCenter.Y;
            position.X += width * (1 - Zoom / newZoom);
            position.Y += height * (1 - Zoom / newZoom);
            Zoom = newZoom;
        }
        public Vector2 WorldToScreenSpace(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, Transform(viewport));
        }
        public Vector2 ScreenToWorldSpace(Vector2 point)
        {
            Matrix invertedMatrix = Matrix.Invert(Transform(viewport));
            return Vector2.Transform(point, invertedMatrix);
        }
        public Matrix Transform(Viewport viewport)
        {
            int viewportWidth = viewport.Width;
            int viewportHeight = viewport.Height;
            this.viewport = viewport;

            return
                Matrix.CreateTranslation(new Vector3(-Position.X - viewportWidth / 2, -Position.Y - viewportHeight / 2, 0)) * // Translation Matrix
                Matrix.CreateRotationZ(Rotation) * // Rotation Matrix
                Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) * // Scale Matrix
                Matrix.CreateTranslation(new Vector3(viewportWidth / 2, viewportHeight / 2, 0)); // Origin/Offset Matrix
        }


    }
}
