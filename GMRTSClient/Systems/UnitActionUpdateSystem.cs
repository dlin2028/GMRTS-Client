using GMRTSClient.UI.ClientAction;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Systems
{
    class UnitActionUpdateSystem : EntityUpdateSystem
    {
        ComponentMapper<PlayerAction> actionMapper;
        public UnitActionUpdateSystem()
              : base(Aspect.All(typeof(PlayerAction)))
        {

        }
        public override void Initialize(IComponentMapperService mapperService)
        {
            actionMapper = mapperService.GetMapper<PlayerAction>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entityID in ActiveEntities)
            {
                var action = actionMapper.Get(entityID);
                if(action is UnitUnitAction)
                {
                    ((UnitUnitAction)action).Update();
                }
            }
        }
    }
}
