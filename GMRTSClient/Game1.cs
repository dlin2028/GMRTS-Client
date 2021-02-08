using GMRTSClasses;
using GMRTSClient.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GMRTSClient
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Viewport viewport => graphics.GraphicsDevice.Viewport;
        private Camera mainCamera;

        private List<Unit> units;
        private Dictionary<Guid, Unit> unitDic;
        private Dictionary<Guid, ClientAction> actionDic;

        private GameUI gameUI;
        private FrameCounter frameCounter = new FrameCounter();
        private SpriteFont smallFont;

        private SignalRClient client;
        private Stopwatch stopwatch = new Stopwatch();

        private void Client_OnGameStart(DateTime obj)
        {
            ;
            stopwatch.Restart();
        }

        private void Client_SpawnUnit(GMRTSClasses.STCTransferData.UnitSpawnData obj)
        {
            Unit unit = null;
            switch (obj.Type)
            {
                case "Tank":
                    unit = new Tank(obj.ID, Content);
                    break;
                case "Builder":
                    unit = new Builder(obj.ID, Content);
                    break;
                default:
                    throw new Exception();
            }

            units.Add(unit);
            unitDic.Add(unit.ID, unit);
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true; 
            Window.AllowUserResizing = true;
            //uncomment for 3000 FPS ultimate gamer mode
            //IsFixedTimeStep = false;
            //graphics.SynchronizeWithVerticalRetrace = false;

            units = new List<Unit>();
            unitDic = new Dictionary<Guid, Unit>();
            actionDic = new Dictionary<Guid, ClientAction>();

            stopwatch = new Stopwatch();
        }
        
        protected override void Initialize()
        {
            mainCamera = new Camera();

            gameUI = new GameUI(mainCamera, GraphicsDevice, Content);
            Window.ClientSizeChanged += (s, e) => { gameUI = new GameUI(mainCamera, GraphicsDevice, Content) { Actions = gameUI.Actions}; };

            
            client = new SignalRClient("http://localhost:53694/server", a => unitDic[a], TimeSpan.FromMilliseconds(400));
            client.OnGameStart += Client_OnGameStart;
            client.SpawnUnit += Client_SpawnUnit;
            client.OnActionFinish += Client_OnActionFinish;
            Task<bool> startConnectionTask = client.TryStart();
            startConnectionTask.Wait();
            Task.Run(async () =>
            {
                await client.JoinGameByNameAndCreateIfNeeded("aaaaa", Guid.NewGuid().ToString());
                await client.RequestGameStart();
            });


            if (!startConnectionTask.Result)
            {
                Random rng = new Random();
                for (int i = 0; i < 10; i++)
                {
                    units.Add(new ClientOnlyUnit(Content.Load<Texture2D>("Builder"), Content.Load<Texture2D>("SelectionMarker"), new Vector2(rng.Next(-5000, 5000), rng.Next(-5000, 5000)), (float)rng.NextDouble()));
                }
            }

            base.Initialize();
        }

        private void Client_OnActionFinish(GMRTSClasses.STCTransferData.ActionOver obj)
        {
            foreach (var unit in obj.Units)
            {
                gameUI.RemoveAction((UnitAction)actionDic[obj.ActionID], unitDic[unit]);
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            smallFont = Content.Load<SpriteFont>("smallfont");
        }

        protected override void Update(GameTime gameTime)
        {
            #region camera
            var mouseWorldPos = mainCamera.ScreenToWorldSpace(new Vector2(InputManager.MouseState.X, InputManager.MouseState.Y));

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
                    mainCamera.ZoomTowardsPoint(viewport, mouseWorldPos, zoomDelta / 300f);
                }
            }
            #endregion

            InputManager.Update();
            gameUI.Update(units.ToArray(), gameTime);

            foreach (var newAction in gameUI.GetPendingActions())
            {
                actionDic.Add(newAction.ID, newAction);

                var nonmeta = newAction.ToDTONonmetaAction();
                var meta = newAction.ToDTOMetaAction();

                if (meta == null)
                {
                    client.ArbitraryNonmeta(nonmeta);
                }
                else
                {
                    client.ArbitraryMeta(meta);
                }
            }

            foreach (var unit in units)
            {
                unit.Update((ulong)stopwatch.ElapsedMilliseconds);
            }

            base.Update(gameTime); 
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            frameCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);


            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, mainCamera.Transform(viewport));
            foreach (var unit in units)
            {
                unit.Draw(spriteBatch);
            }
            gameUI.DrawWorld(spriteBatch);
            spriteBatch.End();


            spriteBatch.Begin(SpriteSortMode.BackToFront);
            gameUI.Draw(spriteBatch);
            //this should be moved to ui later
            spriteBatch.DrawString(smallFont, string.Format("FPS: {0} \n Money {1} \n Minerals {2}", frameCounter.AverageFramesPerSecond, 5, 5), new Vector2(1, 1), Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
