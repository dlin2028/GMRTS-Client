using GMRTSClient.Unit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GMRTSClient
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Viewport viewport => graphics.GraphicsDevice.Viewport;

        private Camera mainCamera;

        private Texture2D map;
        private Tank tank;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true; 
            Window.AllowUserResizing = true;
        }
        
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            mainCamera = new Camera();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            tank = new Tank(Vector2.Zero, 0f, Content.Load<Texture2D>("Tank"), Content.Load<Texture2D>("SelectionMarker"));
            map = Content.Load<Texture2D>("Map");
        }
        Vector2 mouseWorldPos;
        protected override void Update(GameTime gameTime)
        {
            mouseWorldPos = mainCamera.ScreenToWorldSpace(new Vector2(InputManager.MouseState.X, InputManager.MouseState.Y));

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
                    mainCamera.ZoomTowardsPoint(viewport, mouseWorldPos, (zoomDelta) / 1000f);
                }
            }


            // TODO: Add your update logic here
            InputManager.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, mainCamera.Transform(viewport));
            //spriteBatch.Begin();
            spriteBatch.Draw(map, new Rectangle(-400, -400, 1000, 1000), Color.White);
            //spriteBatch.Draw(tank, new Rectangle(10, 10, 50, 50), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
