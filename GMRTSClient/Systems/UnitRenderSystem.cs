﻿using GMRTSClient.Units;
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

        public override void Initialize(IComponentMapperService mapperService)
        {
            transformMapper = mapperService.GetMapper<Transform2>();
            spriteMapper = mapperService.GetMapper<Unit>();
        }

        public UnitRenderSystem(GraphicsDevice graphics)
            : base(Aspect.All(typeof(Unit), typeof(Transform2)))
        {
            spriteBatch = new SpriteBatch(graphics);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            foreach (var entity in ActiveEntities)
            {
                var unit = spriteMapper.Get(entity);
                var transform = transformMapper.Get(entity);
                spriteBatch.Draw(unit.Sprite, transform);
                
            }

            spriteBatch.End();
        }
    }
}