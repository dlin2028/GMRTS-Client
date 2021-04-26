using GMRTSClasses;
using GMRTSClasses.CTSTransferData;
using GMRTSClient.ClientAction;
using GMRTSClient.Component;
using GMRTSClient.Component.Unit;
using GMRTSClient.UI;
using GMRTSClient.UI.ClientAction;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace GMRTSClient.Systems
{
    class ServerUpdateSystem : EntityUpdateSystem
    {
        private static ServerUpdateSystem instance;
        public static ServerUpdateSystem Instance
        {
            get
            {
                return instance;
            }
        }

        private SignalRClient client;

        public Stopwatch Stopwatch { get; set; }

        private List<Component.Unit.Unit> units;

        private Dictionary<Guid, Unit> unitDic;
        private Dictionary<Guid, PlayerAction> actionDic;
        private Dictionary<Guid, FactoryOrder> factoryActionDic;

        private ContentManager content;

        private ComponentMapper<Component.Unit.Unit> unitMapper;
        private ComponentMapper<DTOActionData> actionDataMapper;
        private ComponentMapper<Factory> factoryMapper;
        private ComponentMapper<PlayerAction> actionMapper;

        private readonly GameUI gameui;

        public ServerUpdateSystem(GameUI gameui, ContentManager content, Stopwatch stopwatch)
            : base(Aspect.One(typeof(Unit), typeof(DTOActionData)))
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                throw new Exception("Systems are singletons");
            }

            this.Stopwatch = stopwatch;
            this.gameui = gameui;
            this.content = content;
            units = new List<Unit>();
            unitDic = new Dictionary<Guid, Unit>();
            actionDic = new Dictionary<Guid, PlayerAction>();
            factoryActionDic = new Dictionary<Guid, FactoryOrder>();

            client = new SignalRClient("http://localhost:53694/server", a => unitDic[a] /* this is beautiful, thanks peter */, TimeSpan.FromMilliseconds(400));
            client.OnGameStart += Client_OnGameStart;
            client.SpawnUnit += Client_SpawnUnit;
            client.OnActionFinish += Client_OnActionFinish;
            client.OnResourceUpdated += Client_OnResourceUpdated;

            gameui.BuildTankButton.Click += (s, a) =>  { EnqueueFactoryOrder(GMRTSClasses.Units.MobileUnitType.Tank); };
            gameui.BuildBuilderButton.Click += (s, a) =>  { EnqueueFactoryOrder(GMRTSClasses.Units.MobileUnitType.Builder); };

            Task<bool> startConnectionTask = client.TryStart();
            startConnectionTask.Wait();
            Task.Run(async () =>
            {
                await client.JoinGameByNameAndCreateIfNeeded("aaaaa", Guid.NewGuid().ToString());
                await client.RequestGameStart();
            });

            Task.Run(async () =>
            {
                await Task.Delay(500);


                if (!startConnectionTask.Result)
                {
                    //add some units for debugging
                    Random rng = new Random();
                    for (int i = 0; i < 10; i++)
                    {
                        var entity = CreateEntity();
                        var transform = new Transform2(rng.Next(-500, 500), rng.Next(-500, 500));
                        transform.Scale = Vector2.One * 0.1f;
                        Unit unit = new Unit(Guid.NewGuid());
                        Builder unitComponent = new Builder(unit, content);


                        entity.Attach(unit);
                        entity.Attach(unitComponent);
                        entity.Attach(transform);
                        entity.Attach(new ClientOnlyUnit(unit, transform, unitComponent, rng.Next(0, 100)));
                        entity.Attach(unit.Sprite);
                        entity.Attach(new FancyRect(transform, unit.Sprite.TextureRegion.Size));
                        entity.Attach(new Selectable());

                        units.Add(unit);
                        unitDic.Add(unit.ID, unit);
                    }

                    { //this is c o o l (i hope)
                        var entity = CreateEntity();
                        var transform = new Transform2(0, 0);
                        transform.Scale = Vector2.One * 0.1f;
                        Unit unit = new Unit(Guid.NewGuid());
                        Factory unitComponent = new Factory(unit, content);

                        entity.Attach(unit);
                        entity.Attach(unitComponent);
                        entity.Attach(transform);
                        entity.Attach(new ClientOnlyUnit(unit, transform, unitComponent, rng.Next(0, 100)));
                        entity.Attach(unit.Sprite);
                        entity.Attach(new FancyRect(transform, unit.Sprite.TextureRegion.Size));
                        entity.Attach(new Selectable());

                        units.Add(unit);
                        unitDic.Add(unit.ID, unit);
                    }
                    stopwatch.Restart();
                }
            });
        }

        private void Client_OnResourceUpdated(GMRTSClasses.STCTransferData.ResourceUpdate obj)
        {
            if(obj.ResourceType == GMRTSClasses.STCTransferData.ResourceType.Mineral)
            {
                gameui.Minerals = new Changing<float>(obj.Value.Start, obj.Value.Change, FloatChanger.FChanger, obj.Value.StartTime);
            }
            else
            {
                gameui.Gold = new Changing<float>(obj.Value.Start, obj.Value.Change, FloatChanger.FChanger, obj.Value.StartTime); ;
            }
        }

        private void EnqueueFactoryOrder(GMRTSClasses.Units.MobileUnitType unitType)
        {
            foreach (var entityID in SelectionSystem.Instance.SelectedEntities)
            {
                if(factoryMapper.Has(entityID))
                {
                    var factory = factoryMapper.Get(entityID);
                    var entity = CreateEntity();
                    var order = new FactoryEnqueueOrder(factory.Unit.ID, unitType);
                    entity.Attach(order);
                    entity.Attach(new DTOActionData(order));
                }
            }
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            unitMapper = mapperService.GetMapper<Unit>();
            actionDataMapper = mapperService.GetMapper<DTOActionData>();
            factoryMapper = mapperService.GetMapper<Factory>();
            actionMapper = mapperService.GetMapper<PlayerAction>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entityId in ActiveEntities)
            {
                if (unitMapper.Has(entityId))
                {
                    var unit = unitMapper.Get(entityId);

                    unit.Update((ulong)Stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    var actionData = actionDataMapper.Get(entityId);

                    var nonmeta = actionData.DTONonmetaAction;
                    var meta = actionData.DTOMetaAction;
                    var factory = actionData.DTOFactoryOrder;

                    if (nonmeta != null)
                    {
                        actionDic.Add(actionData.Action.ID, actionData.Action);
                        client.ArbitraryNonmeta(nonmeta);
                    }
                    if(factory != null)
                    {
                        client.FactoryAct(factory);
                    }
                    if(meta != null)
                    {
                        client.ArbitraryMeta(meta);
                    }
                    actionDataMapper.Delete(entityId);

                    //maybe destroy empty entities here
                    if(!actionMapper.Has(entityId))
                    {
                        DestroyEntity(entityId);
                    }
                }
            }
        }


        /// <summary>
        /// Called when the server starts a game
        /// </summary>
        /// <param name="obj">The time the game starts i think</param>
        private void Client_OnGameStart(DateTime obj)
        {
            Stopwatch.Restart();
        }
        /// <summary>
        /// Called when the server spawns a unit
        /// </summary>
        /// <param name="obj">The unit spawn data</param>
        private void Client_SpawnUnit(GMRTSClasses.STCTransferData.UnitSpawnData obj)
        {
            Unit unit = new Unit(obj.ID);

            var entity = CreateEntity();
            var transform = new Transform2();
            transform.Scale = Vector2.One * 0.1f;

            UnitComponent unitComponent;

            switch (obj.Type)
            {
                case "Tank":
                    unitComponent = new Tank(unit, content);
                    entity.Attach((Tank)unitComponent);
                    break;
                case "Builder":
                    unitComponent = new Builder(unit, content);
                    entity.Attach((Builder)unitComponent);
                    break;
                case "Factory":
                    transform.Scale = Vector2.One * 0.001f;
                    unitComponent = new Factory(unit, content);
                    entity.Attach((Factory)unitComponent);
                    break;
                case "Mine":
                    unitComponent = new Mine(unit, content);
                    entity.Attach((Mine)unitComponent);
                    break;
                case "Supermarket":
                    unitComponent = new Supermarket(unit, content);
                    entity.Attach((Supermarket)unitComponent);
                    break;
                default:
                    throw new Exception("Invalid Unit Type");
            }

            entity.Attach(unit);
            entity.Attach(unitComponent.Sprite);
            entity.Attach(transform);
            entity.Attach(new FancyRect(transform, unitComponent.Sprite.TextureRegion.Size));
            entity.Attach(new Selectable());
             
            units.Add(unit);
            unitDic.Add(unit.ID, unit);
        }

        /// <summary>
        /// Updates the UI when a unitaction is finished
        /// </summary>
        /// <param name="obj">The actionover data</param>
        private void Client_OnActionFinish(GMRTSClasses.STCTransferData.ActionOver obj)
        {
            foreach (var id in obj.Units)
            {
                unitDic[id].Orders.Remove((UnitAction)actionDic[obj.ActionID]);
                ((UnitAction)actionDic[obj.ActionID]).Units.Remove(unitDic[id]);
            }
        }

    }
}
