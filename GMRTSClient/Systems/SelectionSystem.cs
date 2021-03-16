using GMRTSClient.Component;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Systems
{
    class SelectionSystem : EntityDrawSystem
    {
        private readonly SpriteBatch spriteBatch;
        private readonly OrthographicCamera camera;
        private ComponentMapper<Selectable> selectableMapper;
        private ComponentMapper<FancyRect> rectMapper;
        private ComponentMapper<Transform2> transMapper;
        private ComponentMapper<Sprite> spriteMapper;

        private Texture2D selectionTexture;
        private Texture2D pixel;
        private bool dragging;

        private MouseListener mouseListener;

        private Point selectionBegin;
        private Rectangle selectionRect;


        public SelectionSystem(ContentManager content, GraphicsDevice graphics, SpriteBatch spriteBatch, OrthographicCamera camera)
            : base(Aspect.All(typeof(Selectable), typeof(FancyRect), typeof(Transform2), typeof(Sprite)))
        {
            this.spriteBatch = spriteBatch;
            this.camera = camera;

            pixel = new Texture2D(graphics, 1, 1);
            pixel.SetData(new[] { Color.White });
            selectionTexture = content.Load<Texture2D>("SelectionMarker");

            mouseListener = new MouseListener();

            mouseListener.MouseDragStart += MouseListener_MouseDragStart;
            mouseListener.MouseDrag += MouseListener_MouseDrag;
            mouseListener.MouseDragEnd += MouseListener_MouseDragEnd;
        }
        public override void Initialize(IComponentMapperService mapperService)
        {
            selectableMapper = mapperService.GetMapper<Selectable>();
            rectMapper = mapperService.GetMapper<FancyRect>();
            transMapper = mapperService.GetMapper<Transform2>();
            spriteMapper = mapperService.GetMapper<Sprite>();
        }
        private Rectangle createRectangle(Point a, Point b)
        {
            return new Rectangle(Math.Min(a.X, b.X),
               Math.Min(a.Y, b.Y),
               Math.Max(Math.Abs(a.X - b.X), 1),
               Math.Max(Math.Abs(a.Y - b.Y), 1));
        }

        private void MouseListener_MouseDragStart(object sender, MouseEventArgs e)
        {
            if(e.CurrentState.LeftButton != ButtonState.Pressed)
                return;

            selectionRect = new Rectangle();
            selectionBegin = camera.ScreenToWorld(e.Position.ToVector2()).ToPoint();
            dragging = true;
        }

        private void MouseListener_MouseDrag(object sender, MouseEventArgs e)
        {
            if (e.CurrentState.LeftButton != ButtonState.Pressed)
            {
                MouseListener_MouseDragEnd(sender, e);
                return;
            }

            selectionRect = createRectangle(selectionBegin, camera.ScreenToWorld(e.Position.ToVector2()).ToPoint());
        }

        private void MouseListener_MouseDragEnd(object sender, MouseEventArgs e)
        {
            dragging = false;

            KeyboardStateExtended keyboardState = KeyboardExtended.GetState();

            foreach (var entityId in ActiveEntities)
            {
                var selectable = selectableMapper.Get(entityId);
                var rect = rectMapper.Get(entityId);

                if (rect.Intersecting(selectionRect))
                {
                    if (keyboardState.IsShiftDown())
                    {
                        selectable.Selected = !selectable.Selected;
                    }
                    else
                    {
                        selectable.Selected = true;
                    }
                }
                else
                {
                    if (!keyboardState.IsShiftDown())
                    {
                        selectable.Selected = false;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            mouseListener.Update(gameTime);
            if (dragging)
            {
                spriteBatch.Draw(pixel, selectionRect, Color.Green);
            }

            foreach (var entityId in ActiveEntities)
            {
                var selectable = selectableMapper.Get(entityId);
                var transform = transMapper.Get(entityId);
                var rect = rectMapper.Get(entityId);
                var sprite = spriteMapper.Get(entityId);

                if (selectable.Selected)
                {
                    spriteBatch.Draw(selectionTexture, new Rectangle((int)transform.WorldPosition.X, (int)transform.WorldPosition.Y, (int)(transform.WorldScale.X * sprite.TextureRegion.Width), (int)(transform.WorldScale.Y * sprite.TextureRegion.Height)), null, Color.White, -transform.WorldRotation, new Vector2(selectionTexture.Width / 2, selectionTexture.Height / 2), SpriteEffects.None, 0);
                }
            }
        }

    }
}
