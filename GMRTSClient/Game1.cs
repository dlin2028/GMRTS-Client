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

        private Texture2D pixel;

        private void Client_OnGameStart(DateTime obj)
        {
            stopwatch.Restart();
        }

        private void Client_SpawnUnit(GMRTSClasses.STCTransferData.UnitSpawnData obj)
        {
            Unit unit = null;
            switch (obj.Type)
            {
                case "Tank":
                    new Tank(obj.ID, 0f, 0.01f, Content.Load<Texture2D>("Tank"), Content.Load<Texture2D>("SelectionMarker"));
                    break;
                case "Builder":
                    unit = new Builder(obj.ID, 0f, 0.01f, Content.Load<Texture2D>("Builder"), Content.Load<Texture2D>("SelectionMarker"));
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
            IsFixedTimeStep = false;
            //graphics.SynchronizeWithVerticalRetrace = false;

            units = new List<Unit>();
            unitDic = new Dictionary<Guid, Unit>();
            actionDic = new Dictionary<Guid, ClientAction>();

            stopwatch = new Stopwatch();
        }
        
        protected override void Initialize()
        {
            mainCamera = new Camera();

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            
            gameUI = new GameUI(mainCamera, GraphicsDevice, pixel, Content.Load<Texture2D>("Circle"));
            Window.ClientSizeChanged += (s, e) => { gameUI = new GameUI(mainCamera, GraphicsDevice, pixel, Content.Load<Texture2D>("Circle")) { Actions = gameUI.Actions}; };

            
            client = new SignalRClient("http://localhost:61337/server", a => unitDic[a], TimeSpan.FromMilliseconds(400));
            client.OnGameStart += Client_OnGameStart;
            client.SpawnUnit += Client_SpawnUnit;
            client.TryStart().Wait();
            Task.Run(async () =>
            {
                await client.JoinGameByNameAndCreateIfNeeded("aaaaa", Guid.NewGuid().ToString());
                await client.RequestGameStart();
            });


            Random rng = new Random();
            for (int i = 0; i < 10; i++)
            {
                units.Add(new ClientOnlyUnit(new Vector2(rng.Next(-50, 50), rng.Next(-500, 500)), (float)rng.NextDouble(), 0.1f, Content.Load<Texture2D>("Tank"), Content.Load<Texture2D>("SelectionMarker")));
            }

            base.Initialize();
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
                    mainCamera.ZoomTowardsPoint(viewport, mouseWorldPos, (zoomDelta) / 100f);
                }
            }
            #endregion

            InputManager.Update();
            gameUI.Update(units.ToArray(), gameTime);

            foreach (var newAction in gameUI.GetPendingActions())
            {
                actionDic.Add(newAction.ID, newAction);
                switch (newAction.ActionType)
                {
                    case ActionType.None:
                        break;
                    case ActionType.Move:
                        client.MoveAction(new GMRTSClasses.CTSTransferData.UnitGround.MoveAction() { ActionID = newAction.ID, Position = new System.Numerics.Vector2(((UnitAction)newAction).Position.X, ((UnitAction)newAction).Position.Y), UnitIDs = ((UnitAction)newAction).Units.Select(x => x.ID).ToList(), RequeueOnCompletion = false});
                        break;
                    case ActionType.Attack:
                        client.AttackAction(new GMRTSClasses.CTSTransferData.UnitUnit.AttackAction() { ActionID = newAction.ID, Target = ((UnitUnitAction)newAction).Target.ID, UnitIDs = ((UnitAction)newAction).Units.Select(x => x.ID).ToList()});
                        break;
                    case ActionType.Assist:
                        client.AssistAction(new GMRTSClasses.CTSTransferData.UnitUnit.AssistAction() { ActionID = newAction.ID, Target = ((UnitUnitAction)newAction).Target.ID, UnitIDs = ((UnitAction)newAction).Units.Select(x => x.ID).ToList() });
                        break;
                    case ActionType.Patrol:
                        client.MoveAction(new GMRTSClasses.CTSTransferData.UnitGround.MoveAction() { ActionID = newAction.ID, Position = new System.Numerics.Vector2(((UnitAction)newAction).Position.X, ((UnitAction)newAction).Position.Y), UnitIDs = ((UnitAction)newAction).Units.Select(x => x.ID).ToList(), RequeueOnCompletion = true });
                        break;
                    case ActionType.Replace:

                        //awful disgusting code pls delete asap
                        GMRTSClasses.CTSTransferData.ClientAction replacementAction = null;
                        ClientAction localReplacementAction = ((ReplaceAction)newAction).NewAction;

                        switch (((ReplaceAction)newAction).NewAction.ActionType)
                        {
                            case ActionType.None:
                                break;
                            case ActionType.Move:
                                replacementAction = new GMRTSClasses.CTSTransferData.UnitGround.MoveAction() { ActionID = newAction.ID, Position = new System.Numerics.Vector2(((UnitAction)localReplacementAction).Position.X, ((UnitAction)localReplacementAction).Position.Y), UnitIDs = ((UnitAction)localReplacementAction).Units.Select(x => x.ID).ToList(), RequeueOnCompletion = false };
                                break;
                            case ActionType.Attack:
                                replacementAction = new GMRTSClasses.CTSTransferData.UnitUnit.AttackAction() { ActionID = newAction.ID, Target = ((UnitUnitAction)localReplacementAction).Target.ID, UnitIDs = ((UnitAction)localReplacementAction).Units.Select(x => x.ID).ToList() };
                                break;
                            case ActionType.Assist:
                                replacementAction = new GMRTSClasses.CTSTransferData.UnitUnit.AssistAction() { ActionID = newAction.ID, Target = ((UnitUnitAction)localReplacementAction).Target.ID, UnitIDs = ((UnitAction)localReplacementAction).Units.Select(x => x.ID).ToList() };
                                break;
                            case ActionType.Patrol:
                                replacementAction = new GMRTSClasses.CTSTransferData.UnitGround.MoveAction() { ActionID = newAction.ID, Position = new System.Numerics.Vector2(((UnitAction)localReplacementAction).Position.X, ((UnitAction)localReplacementAction).Position.Y), UnitIDs = ((UnitAction)localReplacementAction).Units.Select(x => x.ID).ToList(), RequeueOnCompletion = true };
                                break;
                            default:
                                throw new Exception("action type invalid");
                        }

                        client.ReplaceAction(new GMRTSClasses.CTSTransferData.MetaActions.ReplaceAction() { AffectedUnits = replacementAction.UnitIDs, NewAction = replacementAction, TargetActionID = ((ReplaceAction)newAction).OldId });
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
