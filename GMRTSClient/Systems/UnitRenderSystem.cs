using GMRTSClient.Component.Unit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Systems
{
    internal class UnitRenderSystem : EntityDrawSystem
    {
        private readonly SpriteBatch spriteBatch;

        private ComponentMapper<Transform2> transformMapper;
        private ComponentMapper<Unit> spriteMapper;

        private Texture2D pixel;

        public override void Initialize(IComponentMapperService mapperService)
        {
            transformMapper = mapperService.GetMapper<Transform2>();
            spriteMapper = mapperService.GetMapper<Unit>();
        }

        public UnitRenderSystem(SpriteBatch spriteBatch, GraphicsDevice graphics)
            : base(Aspect.All(typeof(Unit), typeof(Transform2)))
        {
            this.spriteBatch = spriteBatch;

            pixel = new Texture2D(graphics, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                var unit = spriteMapper.Get(entity);
                var transform = transformMapper.Get(entity);
                spriteBatch.Draw(unit.Sprite, transform);

                Vector2 size = unit.Sprite.TextureRegion.Size * transform.Scale;
                var healthBarSize = new Vector2(Math.Max(size.X, size.Y) , 10);

                spriteBatch.Draw(pixel, new Rectangle((int)(transform.WorldPosition.X - healthBarSize.X/2f), (int)(transform.WorldPosition.Y + Math.Max(size.X, size.Y) * 1.414 / 2f), (int)healthBarSize.X, (int)healthBarSize.Y), unit.Color);
            }
        }
    }
}
