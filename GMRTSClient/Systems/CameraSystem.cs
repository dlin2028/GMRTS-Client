using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Input.InputListeners;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Systems
{
    class CameraSystem : EntityUpdateSystem //not really updating any entities but that's fine right?
    {
        private OrthographicCamera camera;
        private MouseListener mouseListener;

        private float newZoom;
        private Vector2 newPos;

        public CameraSystem(OrthographicCamera camera)
            :base(Aspect.All())
        {
            this.camera = camera;
            newZoom = camera.Zoom;
            mouseListener = new MouseListener();
            mouseListener.MouseDrag += MouseListener_MouseDrag;
            mouseListener.MouseWheelMoved += MouseListener_MouseWheelMoved;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            //heh heh...
        }
        public void Pan(Vector2 distance, bool worldSpace = false)
        {
            if (worldSpace)
                newPos -= distance;
            else
                newPos -= distance * (1 / camera.Zoom);

            camera.Position = newPos;
        }
        public void ZoomTowardsPoint(Vector2 point, float deltaZoom)
        {
            newZoom = Math.Clamp(camera.Zoom + camera.Zoom * deltaZoom, camera.MinimumZoom, camera.MaximumZoom);

            var width = point.X - camera.Center.X;
            var height = point.Y - camera.Center.Y;
            newPos = camera.Position + new Vector2(width * (1 - camera.Zoom / newZoom), height * (1 - camera.Zoom / newZoom));
        }

        private void MouseListener_MouseDrag(object sender, MouseEventArgs e)
        {
            if (e.Button != MonoGame.Extended.Input.MouseButton.Middle) return;

            var panDelta = e.CurrentState.Position - e.PreviousState.Position;
            Pan(panDelta.ToVector2());
        }

        private void MouseListener_MouseWheelMoved(object sender, MouseEventArgs e)
        {
            var zoomDelta = e.CurrentState.ScrollWheelValue - e.PreviousState.ScrollWheelValue;
            if (zoomDelta != 0)
            {
                ZoomTowardsPoint(camera.ScreenToWorld(e.Position.ToVector2()), zoomDelta / 300f);
            }
        }

        public override void Update(GameTime gameTime)
        {
            mouseListener.Update(gameTime);

            camera.Zoom = MathHelper.Lerp(camera.Zoom, newZoom, 0.3f);
            camera.Position = Vector2.Lerp(camera.Position, newPos, 0.3f);
        }
    }
}
