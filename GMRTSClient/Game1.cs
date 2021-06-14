using System.Diagnostics;
using GMRTSClient.Systems;
using GMRTSClient.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.ViewportAdapters;
using Myra;
using Myra.Graphics2D.UI;

namespace GMRTSClient
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private World world;
        private OrthographicCamera camera;
        private Desktop desktop;
        GameUI gameUI;
        private UIStatus uiStatus;

        //TEMPORARY
        //public FrameCounter frameCounter;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //frameCounter = new FrameCounter();
            //uncomment for 3000 FPS ultimate gamer mode
            //also works as PC toaster mode
            //IsFixedTimeStep = false;
            //graphics.SynchronizeWithVerticalRetrace = false;
        }

        protected override void Initialize()
        {
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 1280, 720);
            camera = new OrthographicCamera(viewportAdapter);
            camera.MinimumZoom = 0.05f;
            camera.MaximumZoom = 2.00f;

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            MyraEnvironment.Game = this;
            gameUI = new GameUI();
            desktop = new Desktop();
            desktop.Root = gameUI;
            var stopwatch = new Stopwatch();

            uiStatus = new UIStatus(gameUI.CurrentAction, gameUI.CurrentBuilding, desktop.IsMouseOverGUI);

            world = new WorldBuilder()
                   .AddSystem(new CameraSystem(camera))
                   .AddSystem(new ServerUpdateSystem(gameUI, Content, stopwatch))
                   .AddSystem(new UnitUpdateSystem(stopwatch))
                   .AddSystem(new UnitRenderSystem(spriteBatch, GraphicsDevice))
                   .AddSystem(new RenderSystem(spriteBatch))
                   .AddSystem(new UnitActionCreationSystem(uiStatus, gameUI, camera, Content))
                   .AddSystem(new UnitActionEditSystem(camera))
                   .AddSystem(new UnitActionUpdateSystem())
                   .AddSystem(new UnitActionRenderSystem(Content, GraphicsDevice, spriteBatch))
                   .AddSystem(new SelectionSystem(Content, GraphicsDevice, spriteBatch, camera, uiStatus))
                   .AddSystem(new UIUpdateSystem(gameUI, uiStatus)) //for some reason the mouseListener OnDragEnd happens here first
                   .AddSystem(new MapRenderSystem(Content, GraphicsDevice, camera))
                   .Build();

            Components.Add(world);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            uiStatus.Update(gameUI.CurrentAction, gameUI.CurrentBuilding, desktop.IsMouseOverGUI);
            world.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //frameCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            //gameUI.FPSLabel.Text = "FPS: " + frameCounter.CurrentFramesPerSecond;
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var transformMatrix = camera.GetViewMatrix();
            spriteBatch.Begin(transformMatrix: transformMatrix);

            world.Draw(gameTime);

            base.Draw(gameTime);
            spriteBatch.End();

            desktop.Render();
        }
    }
}
