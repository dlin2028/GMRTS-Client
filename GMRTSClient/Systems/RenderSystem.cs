using GMRTSClient.Component.Unit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;
using System;

namespace GMRTSClient.Systems
{
    internal class RenderSystem : EntityDrawSystem
    {
        private readonly SpriteBatch spriteBatch;

        private ComponentMapper<Transform2> transformMapper;
        private ComponentMapper<Sprite> spriteMapper;

        public override void Initialize(IComponentMapperService mapperService)
        {
            transformMapper = mapperService.GetMapper<Transform2>();
            spriteMapper = mapperService.GetMapper<Sprite>();
        }

        public RenderSystem(SpriteBatch spriteBatch)
            :base(Aspect.All(typeof(Unit), typeof(Sprite), typeof(Transform2)))
        {
            this.spriteBatch = spriteBatch;
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                var sprite = spriteMapper.Get(entity);
                var transform = transformMapper.Get(entity);
                float Z = 0;
                transform.Position = new Vector2((transform.Position.X - Z) / (float)Math.Sqrt(2), (transform.Position.X + 2 * transform.Position.Y + Z) / (float)Math.Sqrt(6));
                spriteBatch.Draw(sprite, transform);
            }
        }
    }
}