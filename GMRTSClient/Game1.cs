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

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //uncomment for 3000 FPS ultimate gamer mode
            //also works as PC toaster mode
            //IsFixedTimeStep = false;
            //graphics.SynchronizeWithVerticalRetrace = false;
        }

        protected override void Initialize()
        {
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            camera = new OrthographicCamera(viewportAdapter);
            camera.MinimumZoom = 0.005f;
            camera.MaximumZoom = 0.65f;

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

            uiStatus = new UIStatus(gameUI.CurrentAction, gameUI.CurrentBuilding, desktop.IsMouseOverGUI);

            world = new WorldBuilder()
                   .AddSystem(new CameraSystem(camera))
                   .AddSystem(new ServerUpdateSystem(Content))
                   .AddSystem(new UnitUpdateSystem())
                   .AddSystem(new UnitRenderSystem(spriteBatch))
                   .AddSystem(new RenderSystem(spriteBatch))
                   .AddSystem(new UnitActionSystem(uiStatus, camera, Content))
                   .AddSystem(new ActionRenderSystem(Content, GraphicsDevice, spriteBatch))
                   .AddSystem(new SelectionSystem(Content, GraphicsDevice, spriteBatch, camera, uiStatus))
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
