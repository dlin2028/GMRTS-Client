using GMRTSClient.Units;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Systems
{
    class UnitSystem : EntityUpdateSystem
    {
        public ulong CurrentMilliseconds { get; set; }

        private ComponentMapper<Transform2> transformMapper;
        private ComponentMapper<Unit> unitMapper;

        public UnitSystem()
           : base(Aspect.All(typeof(Transform2), typeof(Unit)))
        {
            CurrentMilliseconds = 0;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            transformMapper = mapperService.GetMapper<Transform2>();
            unitMapper = mapperService.GetMapper<Unit>();
        }

        public override void Update(GameTime gameTime)
        {
            CurrentMilliseconds += (ulong)gameTime.ElapsedGameTime.TotalMilliseconds;
            foreach (var entityId in ActiveEntities)
            {
                var transform = transformMapper.Get(entityId);
                var unit = unitMapper.Get(entityId);

                unit.Update(CurrentMilliseconds);
                transform.Position = new Vector2(unit.Position.Value.X, unit.Position.Value.Y);
            }
        }
    }
}
