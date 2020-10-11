using GMRTSClient.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GMRTSClient
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Viewport viewport => graphics.GraphicsDevice.Viewport;

        private Camera mainCamera;

        private Texture2D map;

        List<Tank> tanks = new List<Tank>();

        private GameUI gameUI;

        private FrameCounter frameCounter = new FrameCounter();
        SpriteFont smallFont;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true; 
            Window.AllowUserResizing = true;
            IsFixedTimeStep = false;
            //graphics.SynchronizeWithVerticalRetrace = false;
        }
        
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            mainCamera = new Camera();
            gameUI = new GameUI(mainCamera, GraphicsDevice);
            Window.ClientSizeChanged += (s, e) => { gameUI = new GameUI(mainCamera, GraphicsDevice); };
            base.Initialize();
        }
        Texture2D pixel;
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            smallFont = Content.Load<SpriteFont>("smallfont");

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            Random rng = new Random();
            for (int i = 0; i < 50; i++)
            {
                tanks.Add(new Tank(new Vector2(rng.Next(-500, 500), rng.Next(-500, 500)), 0f, 0.01f, Content.Load<Texture2D>("Tank"), Content.Load<Texture2D>("SelectionMarker")));
            }
            map = Content.Load<Texture2D>("Map");
        }
        protected override void Update(GameTime gameTime)
        {
            #region camera
            var mouseWorldPos = mainCamera.ScreenToWorldSpace(new Vector2(InputManager.MouseState.X, InputManager.MouseState.Y));

            // Debug.Print(InputManager.MouseState.MiddleButton.ToString());
            if(InputManager.MouseState.MiddleButton == ButtonState.Pressed)
            {
                var panDelta = InputManager.MouseState.Position - InputManager.LastMouseState.Position;
                mainCamera.Pan(panDelta.ToVector2());
            }
            else
            {
                var zoomDelta = InputManager.MouseState.ScrollWheelValue - InputManager.LastMouseState.ScrollWheelValue;
                if (zoomDelta != 0)
                {
                    mainCamera.ZoomTowardsPoint(viewport, mouseWorldPos, (zoomDelta) / 100f);
                }
            }
            #endregion

            // TODO: Add your update logic here
            InputManager.Update();
            foreach (var tank in tanks)
            {
                //tank.Update();
            }
            //tank.Update((ulong)gameTime.ElapsedGameTime.TotalMilliseconds);
            gameUI.Update(tanks.ToArray()) ;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            frameCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);


            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, mainCamera.Transform(viewport));
            //spriteBatch.Begin();
            //spriteBatch.Draw(map, new Rectangle(-400, -400, 1000, 1000), Color.White);

            foreach (var tank in tanks)
            {
                tank.Draw(spriteBatch);
                //tank.DrawSelectionRect(spriteBatch, pixel);
            }
            gameUI.DrawSelectionRect(spriteBatch);

            spriteBatch.End();


            spriteBatch.Begin(SpriteSortMode.BackToFront);
            gameUI.Draw(spriteBatch);
            spriteBatch.DrawString(smallFont, string.Format("FPS: {0}", frameCounter.AverageFramesPerSecond), new Vector2(1, 1), Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
