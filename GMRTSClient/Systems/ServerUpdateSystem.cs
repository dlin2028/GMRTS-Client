using GMRTSClasses;
using GMRTSClasses.CTSTransferData;
using GMRTSClasses.Units;
using GMRTSClient.UI.ClientActions;
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

        private List<Unit> units;

        private Dictionary<Guid, GMRTSClient.Components.Unit.Unit> unitDic;
        private Dictionary<Guid, PlayerAction> actionDic;

        private ContentManager content;

        private ComponentMapper<Unit> unitMapper;
        private ComponentMapper<DTOActionData> actionMapper;


        public ServerUpdateSystem(ContentManager content)
            : base(Aspect.One(typeof(Unit), typeof(DTOActionData)))
        {
            this.content = content;
            units = new List<Unit>();

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


            if (!startConnectionTask.Result)
            {
                //add some units for debugging
                Random rng = new Random();
                for (int i = 0; i < 10; i++)
                {
                    //this could be in loadcontent?
                    units.Add(new GMRTSClient.Components.Unit.ClientOnlyUnit(content));
                }
            }

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

                    if (meta == null)
                    {
                        client.ArbitraryNonmeta(nonmeta);
                    }
                    else
                    {
                        client.ArbitraryMeta(meta);
                    }
                    actionMapper.Delete(entityId);
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
            GMRTSClient.Components.Unit.Unit unit;
            switch (obj.Type)
            {
                case "Tank":
                    unit = new GMRTSClient.Components.Unit.Tank(obj.ID, content);
                    break;
                case "Builder":
                    unit = new GMRTSClient.Components.Unit.Builder(obj.ID, content);
                    break;
                default:
                    throw new Exception();
            }

            var entity = CreateEntity();
            entity.Attach(unit);
            entity.Attach(new Transform2());

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
