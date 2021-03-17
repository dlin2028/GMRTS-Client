using GMRTSClient.Component.Unit;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Systems
{
    class UnitUpdateSystem : EntityUpdateSystem
    {
        private ComponentMapper<Transform2> transformMapper;
        private ComponentMapper<Unit> unitMapper;

        public UnitUpdateSystem()
           : base(Aspect.All(typeof(Transform2), typeof(Unit)))
        {

        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            transformMapper = mapperService.GetMapper<Transform2>();
            unitMapper = mapperService.GetMapper<Unit>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entityId in ActiveEntities)
            {
                var transform = transformMapper.Get(entityId);
                var unit = unitMapper.Get(entityId);

                unit.Update((ulong)gameTime.TotalGameTime.TotalMilliseconds);

                transform.Position = new Vector2(unit.Position.Value.X, unit.Position.Value.Y);
                transform.Rotation = unit.Rotation.Value;
            }
        }
    }
}
