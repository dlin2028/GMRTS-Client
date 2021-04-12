﻿using GMRTSClient.Component;
using GMRTSClient.UI;
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
    internal class Unsubscriber<Selectable> : IDisposable
    {
        private List<IObserver<Selectable>> _observers;
        private IObserver<Selectable> _observer;

        internal Unsubscriber(List<IObserver<Selectable>> observers, IObserver<Selectable> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
    class SelectionSystem : EntityDrawSystem, IUpdateSystem, IObservable<Selectable>
    {
        private static SelectionSystem instance;
        public static SelectionSystem Instance
        {
            get
            {
                return instance;
            }
        }
    

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

        private UIStatus uiStatus;

        private List<IObserver<Selectable>> observers;

        public List<int> SelectedEntities;

        public SelectionSystem(ContentManager content, GraphicsDevice graphics, SpriteBatch spriteBatch, OrthographicCamera camera, UIStatus uiStatus)
            : base(Aspect.All(typeof(Selectable), typeof(FancyRect), typeof(Transform2), typeof(Sprite)))
        {
            if(instance == null)
            {
                instance = this;
            }

            observers = new List<IObserver<Selectable>>();
            this.spriteBatch = spriteBatch;
            this.camera = camera;
            this.uiStatus = uiStatus;

            pixel = new Texture2D(graphics, 1, 1);
            pixel.SetData(new[] { Color.White });
            selectionTexture = content.Load<Texture2D>("SelectionMarker");

            SelectedEntities = new List<int>();

            mouseListener = new MouseListener();

            mouseListener.MouseClicked += MouseListener_MouseClicked;
            mouseListener.MouseDragStart += MouseListener_MouseDragStart;
            mouseListener.MouseDrag += MouseListener_MouseDrag;
            mouseListener.MouseDragEnd += MouseListener_MouseDragEnd;
        }

        private void MouseListener_MouseClicked(object sender, MouseEventArgs e)
        {
            if (uiStatus.MouseHovering) return;

            if(!KeyboardExtended.GetState().IsShiftDown())
            {
                foreach (var entityId in ActiveEntities)
                {
                    selectableMapper.Get(entityId).Selected = false;
                }
            }
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
            if(e.Button != MouseButton.Left || uiStatus.MouseHovering)
                return;

            selectionRect = new Rectangle();
            selectionBegin = camera.ScreenToWorld(e.Position.ToVector2()).ToPoint();
            dragging = true;
        }

        private void MouseListener_MouseDrag(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButton.Left || uiStatus.MouseHovering)
                return;

            selectionRect = createRectangle(selectionBegin, camera.ScreenToWorld(e.Position.ToVector2()).ToPoint());
        }

        private void MouseListener_MouseDragEnd(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButton.Left || uiStatus.MouseHovering)
                return;

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

                if(selectable.Selected)
                {
                    SelectedEntities.Add(entityId);
                }
                else
                {
                    SelectedEntities.Remove(entityId);
                }
            }
        }
        public IDisposable Subscribe(IObserver<Selectable> observer)
        {
            // Check whether observer is already registered. If not, add it
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
                // Provide observer with existing data.
                foreach (var item in ActiveEntities)
                    observer.OnNext(selectableMapper.Get(item));
            }
            return new Unsubscriber<Selectable>(observers, observer);
        }

        public override void Draw(GameTime gameTime)
        {
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

        public void Update(GameTime gameTime)
        {
            mouseListener.Update(gameTime);
        }

    }
}
