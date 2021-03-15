using GMRTSClient.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.ViewportAdapters;

namespace GMRTSClient
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private World world;
        private OrthographicCamera camera;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            camera = new OrthographicCamera(viewportAdapter);


            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            world = new WorldBuilder()
                   .AddSystem(new ServerUpdateSystem(Content))
                   .AddSystem(new RenderSystem(spriteBatch))
                   .AddSystem(new ActionRenderSystem(Content, GraphicsDevice, spriteBatch))
                   .AddSystem(new SelectionSystem(Content, GraphicsDevice, spriteBatch, camera))
                   .AddSystem(new UnitRenderSystem(spriteBatch))
                   .AddSystem(new UnitUpdateSystem())
                   .Build();
            Components.Add(world);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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
        }
    }
}
