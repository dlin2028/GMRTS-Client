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

        List<Unit> units;
        Dictionary<Guid, Unit> unitDic;

        private GameUI gameUI;
        private FrameCounter frameCounter = new FrameCounter();
        SpriteFont smallFont;


        Random rng = new Random();

        SignalRClient client;
        Stopwatch stopwatch = new Stopwatch();

        private void Client_OnGameStart(DateTime obj)
        {
            stopwatch.Restart();
        }

        private void Client_SpawnUnit(GMRTSClasses.STCTransferData.UnitSpawnData obj)
        {
            var tank = new Tank(Guid.NewGuid(), 0f, 0.01f, Content.Load<Texture2D>("Tank"), Content.Load<Texture2D>("SelectionMarker"));
            units.Add(tank);
            unitDic.Add(tank.ID, tank);
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true; 
            Window.AllowUserResizing = true;
            IsFixedTimeStep = false;
            //graphics.SynchronizeWithVerticalRetrace = false;

            units = new List<Unit>();
            unitDic = new Dictionary<Guid, Unit>();

            stopwatch = new Stopwatch();
        }
        
        protected override void Initialize()
        {
            mainCamera = new Camera();

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            gameUI = new GameUI(mainCamera, GraphicsDevice, pixel);
            Window.ClientSizeChanged += (s, e) => { gameUI = new GameUI(mainCamera, GraphicsDevice, pixel); };

            
            client = new SignalRClient("http://localhost:61337/", "GameHub", a => unitDic[a], TimeSpan.FromMilliseconds(400));
            client.OnGameStart += Client_OnGameStart;
            client.SpawnUnit += Client_SpawnUnit;
            client.TryStart().Wait();
            Task.Run(async () =>
            {
                await client.JoinGameByNameAndCreateIfNeeded("aaaaa", Guid.NewGuid().ToString());
                await client.RequestGameStart();
            });

            base.Initialize();
        }

        Texture2D pixel;
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            smallFont = Content.Load<SpriteFont>("smallfont");
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

            InputManager.Update();
            gameUI.Update(units.ToArray());

            foreach (var newAction in gameUI.GetPendingActions())
            {
                switch (newAction.ActionType)
                {
                    case ActionType.None:
                        break;
                    case ActionType.Move:
                        client.MoveAction(new GMRTSClasses.CTSTransferData.UnitGround.MoveAction() { ActionID = newAction.ID, Position = new System.Numerics.Vector2(newAction.Position.X, newAction.Position.Y), UnitIDs = newAction.Units.Select(x => x.ID).ToList(), RequeueOnCompletion = false});
                        break;
                    case ActionType.Attack:
                        client.AttackAction(new GMRTSClasses.CTSTransferData.UnitUnit.AttackAction() { ActionID = newAction.ID, Target = ((UnitUnitAction)newAction).Target.ID, UnitIDs = newAction.Units.Select(x => x.ID).ToList()});
                        break;
                    case ActionType.Assist:
                        client.AssistAction(new GMRTSClasses.CTSTransferData.UnitUnit.AssistAction() { ActionID = newAction.ID, Target = ((UnitUnitAction)newAction).Target.ID, UnitIDs = newAction.Units.Select(x => x.ID).ToList() });
                        break;
                    case ActionType.Patrol:
                        client.MoveAction(new GMRTSClasses.CTSTransferData.UnitGround.MoveAction() { ActionID = newAction.ID, Position = new System.Numerics.Vector2(newAction.Position.X, newAction.Position.Y), UnitIDs = newAction.Units.Select(x => x.ID).ToList(), RequeueOnCompletion = true });
                        break;
                    default:
                        break;
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
            spriteBatch.DrawString(smallFont, string.Format("FPS: {0}", frameCounter.AverageFramesPerSecond), new Vector2(1, 1), Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
