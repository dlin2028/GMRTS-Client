using GMRTSClasses;
using GMRTSClasses.CTSTransferData;
using GMRTSClient.ClientAction;
using GMRTSClient.Component;
using GMRTSClient.Component.Unit;
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
        private SignalRClient client;

        private Stopwatch stopwatch = new Stopwatch();

        private List<Component.Unit.Unit> units;

        private Dictionary<Guid, Unit> unitDic;
        private Dictionary<Guid, PlayerAction> actionDic;
        private Dictionary<Guid, FactoryOrder> factoryActionDic;

        private ContentManager content;

        private ComponentMapper<Component.Unit.Unit> unitMapper;
        private ComponentMapper<DTOActionData> actionMapper;


        public ServerUpdateSystem(ContentManager content)
            : base(Aspect.One(typeof(Unit), typeof(DTOActionData)))
        {
            this.content = content;
            units = new List<Unit>();
            unitDic = new Dictionary<Guid, Unit>();
            actionDic = new Dictionary<Guid, PlayerAction>();

            client = new SignalRClient("http://localhost:53694/server", a => unitDic[a] /* this is beautiful, thanks peter */, TimeSpan.FromMilliseconds(400));
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
                        Unit unit = new Unit(Guid.NewGuid());
                        Builder unitComponent = new Builder(unit, content);


                        entity.Attach(unit);
                        entity.Attach(unitComponent);
                        entity.Attach(new ClientOnlyUnit(unit, transform, unitComponent));
                        entity.Attach(unit.Sprite);
                        entity.Attach(transform);
                        entity.Attach(new FancyRect(transform, unit.Sprite.TextureRegion.Size));
                        entity.Attach(new Selectable());

                        units.Add(unit);
                        unitDic.Add(unit.ID, unit);
                    }
                }
            });
        }
        public override void Initialize(IComponentMapperService mapperService)
        {
            unitMapper = mapperService.GetMapper<Unit>();
            actionMapper = mapperService.GetMapper<DTOActionData>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entityId in ActiveEntities)
            {
                if (unitMapper.Has(entityId))
                {
                    var unit = unitMapper.Get(entityId);

                    unit.Update((ulong)stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    var actionData = actionMapper.Get(entityId);

                    var nonmeta = actionData.DTONonmetaAction;
                    var meta = actionData.DTOMetaAction;
                    var factory = actionData.DTOFactoryOrder;

                    if (nonmeta != null)
                    {
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
                    actionMapper.Delete(entityId);

                    //maybe destroy empty entities here
                }
            }
        }


        /// <summary>
        /// Called when the server starts a game
        /// </summary>
        /// <param name="obj">The time the game starts i think</param>
        private void Client_OnGameStart(DateTime obj)
        {
            stopwatch.Restart();
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

            entity.Attach(unit);

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
                default:
                    throw new Exception("Invalid Unit Type");
            }
            entity.Attach(unitComponent);


            entity.Attach(unitComponent.Sprite);
            entity.Attach(transform);
            entity.Attach(new FancyRect(transform, unitComponent.Sprite.TextureRegion.Size));

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
                unitDic[id].Orders.Remove((UnitAction)actionDic[id]);
                ((UnitAction)actionDic[obj.ActionID]).Units.Remove(unitDic[id]);
            }
        }

    }
}
