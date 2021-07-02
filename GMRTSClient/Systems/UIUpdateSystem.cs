using GMRTSClasses.Units;
using GMRTSClient.ClientAction;
using GMRTSClient.Component;
using GMRTSClient.Component.Unit;
using GMRTSClient.UI;
using GMRTSClient.UI.ClientAction;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Input.InputListeners;
using Myra;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMRTSClient.Systems
{
    class UIUpdateSystem : EntityUpdateSystem, IObserver<SelectableData>
    {
        private static UIUpdateSystem instance;
        public static UIUpdateSystem Instance
        {
            get
            {
                return instance;
            }
        }

        private ComponentMapper<Selectable> selectionMapper;
        private ComponentMapper<Component.Unit.Builder> builderMapper;
        private ComponentMapper<Component.Unit.Factory> factoryMapper;
        private readonly GameUI gameUI;
        private readonly UIStatus uiStatus;
        private MouseListener mouseListener;
        private IDisposable unsubscriber;
        private List<(ImageButton, PlayerAction)> queueButtons;
        public UIUpdateSystem(GameUI gameUI, UIStatus uiStatus)
            : base(Aspect.All(typeof(Selectable)))
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                throw new Exception("systems are singletons");
            }
            mouseListener = new MouseListener();
            mouseListener.MouseClicked += RefreshQueue;

            queueButtons = new List<(ImageButton, PlayerAction)>();
            this.gameUI = gameUI;
            this.uiStatus = uiStatus;
            unsubscriber = SelectionSystem.Instance.Subscribe(this);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            selectionMapper = mapperService.GetMapper<Selectable>();
            builderMapper = mapperService.GetMapper<Component.Unit.Builder>();
            factoryMapper = mapperService.GetMapper<Component.Unit.Factory>();
        }

        public override void Update(GameTime gameTime)
        {
            gameUI.Update((ulong)ServerUpdateSystem.Instance.Stopwatch.ElapsedMilliseconds);
        }

        public void OnCompleted()
        {
            unsubscriber.Dispose();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }
        private SelectableData lastValue;
        public void RefreshQueue(object sender, MouseEventArgs e)
        {
            OnNext(lastValue);
        }
        public void RefreshQueue()
        {
            OnNext(lastValue);
        }
        public void OnNext(SelectableData value)
        {
            lastValue = value;
            if (value.SelectedEntityIds.Count == 0 && uiStatus.MouseHovering == false)
            {
                gameUI.CurrentAction = ActionType.None;
                gameUI.BuildMenuFlags = BuildFlags.None;
                while (queueButtons.Count > 0)
                {
                    gameUI.BuildGrid.Widgets.Remove(queueButtons.First().Item1);
                    queueButtons.RemoveAt(0);
                }
                return;
            }

            var firstId = value.SelectedEntityIds.First();
            var currBuildFlags = BuildFlags.All;
            IEnumerable<FactoryOrder> orders = null;
            IEnumerable<BuildAction> actions = null;
            if (factoryMapper.Has(value.SelectedEntityIds.First()))
            {
                var factory = factoryMapper.Get(firstId);
                orders = factory.Orders;
            }
            else
            {
                actions = new List<BuildAction>();
                var builder = builderMapper.Get(firstId);
                actions = builder.Unit.Orders.Where(x => x.ActionType == ActionType.Build).Cast<BuildAction>();
            }

            bool displayQueue = true;
            bool displayBuild = false;
            foreach (var entityID in value.SelectedEntityIds)
            {
                if (displayQueue)
                {
                    if (orders == null)
                    {
                        if (!builderMapper.Has(entityID))
                        {
                            displayQueue = false;
                            currBuildFlags = BuildFlags.None;
                            break;
                        }

                        var builder = builderMapper.Get(entityID);
                        var builderActions = builder.Unit.Orders.Where(x => x.ActionType == ActionType.Build).Cast<BuildAction>();
                        if (!actions.SequenceEqual(builderActions))
                        {
                            displayQueue = false;
                        }
                    }
                    else
                    {
                        if (!factoryMapper.Has(entityID))
                        {
                            displayQueue = false;
                            currBuildFlags = BuildFlags.None;
                            break;
                        }

                        var factory = factoryMapper.Get(entityID);
                        if (!orders.SequenceEqual(factory.Orders))
                        {
                            displayQueue = false;
                        }
                    }
                }

                if (selectionMapper.Get(entityID).Selected)
                {
                    displayBuild = true;
                    if (builderMapper.Has(entityID))
                    {
                        currBuildFlags &= builderMapper.Get(entityID).BuildFlags;
                    }
                    else if (factoryMapper.Has(entityID))
                    {
                        currBuildFlags &= factoryMapper.Get(entityID).BuildFlags;
                    }
                    else
                    {
                        currBuildFlags = BuildFlags.None;
                    }
                }
            }
            if (!displayBuild)
            {
                currBuildFlags = BuildFlags.None;
            }

            while (queueButtons.Count > 0)
            {
                gameUI.BuildGrid.Widgets.Remove(queueButtons.First().Item1);
                queueButtons.RemoveAt(0);
            }

            if (displayQueue)
            {
                if (orders == null)
                {
                    for (int i = 0; i < actions.Count(); i++)
                    {
                        var action = actions.ElementAt(i);
                        if (action.ActionType == ActionType.Build)
                        {
                            var newButton = new ImageButton();

                            newButton.Image = ((BuildAction)action).BuildingType switch
                            {
                                BuildingType.Factory => MyraEnvironment.DefaultAssetManager.Load<TextureRegion>("unitassets/Factory.png"),
                                BuildingType.Mine => MyraEnvironment.DefaultAssetManager.Load<TextureRegion>("unitassets/Mine.png"),
                                BuildingType.Supermarket => MyraEnvironment.DefaultAssetManager.Load<TextureRegion>("unitassets/Market.png")
                            };
                            newButton.PressedImage = MyraEnvironment.DefaultAssetManager.Load<TextureRegion>("buttonassets/patrolPressed.png");
                            newButton.MaxWidth = 100;
                            newButton.MaxHeight = 100;
                            newButton.GridRow = 1;
                            newButton.GridColumn = i;
                            newButton.Id = "BuildFactoryButton";
                            newButton.Click += (s, e) =>
                            {
                                gameUI.BuildGrid.Widgets.Remove((ImageButton)s);
                                UnitActionEditSystem.Instance.DeleteAction(action);
                                OnNext(value);
                            };

                            gameUI.BuildGrid.Widgets.Add(newButton);
                            queueButtons.Add((newButton, action));
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < orders.Count(); i++)
                    {
                        var order = orders.ElementAt(i);
                        var newButton = new ImageButton();

                        if (order is FactoryEnqueueOrder)
                        {
                            newButton.Image = ((FactoryEnqueueOrder)order).UnitType switch
                            {
                                MobileUnitType.Builder => MyraEnvironment.DefaultAssetManager.Load<TextureRegion>("unitassets/Builder.png"),
                                MobileUnitType.Tank => MyraEnvironment.DefaultAssetManager.Load<TextureRegion>("unitassets/Tank.png")
                            };
                            newButton.PressedImage = MyraEnvironment.DefaultAssetManager.Load<TextureRegion>("buttonassets/patrolPressed.png");
                            newButton.MaxWidth = 100;
                            newButton.MaxHeight = 100;
                            newButton.GridRow = 1;
                            newButton.GridColumn = i;
                            newButton.Id = "BuildFactoryButton";
                            int orderIndex = i;
                            newButton.Click += (s, e) =>
                            {
                                foreach (var facId in value.SelectedEntityIds)
                                {
                                    var factory = factoryMapper.Get(facId);
                                    var entity = CreateEntity();
                                    entity.Attach(new DTOActionData(new FactoryCancelOrder(factory.Unit.ID, factory.Orders.ElementAt(orderIndex).ID)));
                                    factory.Orders.RemoveAt(orderIndex);
                                }
                                OnNext(value);
                            };
                            gameUI.BuildGrid.Widgets.Add(newButton);
                            queueButtons.Add((newButton, order));
                        }

                    }
                }
            }

            gameUI.BuildMenuFlags = currBuildFlags;
        }
    }
}
