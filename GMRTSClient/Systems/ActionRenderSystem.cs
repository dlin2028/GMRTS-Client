﻿using GMRTSClient.UI.ClientActions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMRTSClient.Systems
{
    class ActionRenderSystem : EntityDrawSystem
    {
        private ComponentMapper<PlayerAction> actionMapper;
        private SpriteBatch spriteBatch;

        private Texture2D circle;
        private Texture2D pixel;

        public ActionRenderSystem(ContentManager content, GraphicsDevice graphics, SpriteBatch spriteBatch)
            : base(Aspect.All(typeof(PlayerAction)))
        {
            this.spriteBatch = spriteBatch;
            circle = content.Load<Texture2D>("Circle");
            pixel = new Texture2D(graphics, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            actionMapper = mapperService.GetMapper<PlayerAction>();
        }
        public override void Draw(GameTime gameTime)
        {
            foreach (var entityId in ActiveEntities)
            {
                var playerAction = actionMapper.Get(entityId);

                if (playerAction.IsUnitAction)
                {
                    DrawUnitAction((UnitAction)playerAction, gameTime);
                }
            }
        }

        private void DrawUnitAction(UnitAction unitAction, GameTime gameTime)
        {
            if (unitAction.AnimationTime.TotalMilliseconds > 0)
            {
                spriteBatch.Draw(circle, unitAction.Position, null, unitAction.RenderColor * (float)(unitAction.AnimationTime.TotalMilliseconds / 500.0), 0f, new Vector2(circle.Width, circle.Height) / 2, 0.01f, SpriteEffects.None, 0f);
                unitAction.AnimationTime -= gameTime.ElapsedGameTime;
            }
            foreach (var order in unitAction.PrevOrders)
            {
                spriteBatch.Draw(pixel, order.Position, null, unitAction.RenderColor, (float)Math.Atan2(unitAction.Position.Y - order.Position.Y, unitAction.Position.X - order.Position.X), new Vector2(0, (float)pixel.Height / 2f), new Vector2((order.Position - unitAction.Position).Length(), (float)order.Units.Where(x => { var y = x.Orders.Find(order).Next; return y != null && y.Value == unitAction; }).Count() / 2), SpriteEffects.None, 0f);
            }

            if (unitAction.ActionType == ActionType.Patrol)
            {
                var lastOrderNodes = unitAction.Units.Select(x => x.Orders).Select(x => x.Last).Distinct();
                foreach (var lastOrderNode in lastOrderNodes)
                {
                    if (lastOrderNode.List.First(x => x.ActionType == ActionType.Patrol).ID == unitAction.ID)
                    {
                        var lastOrder = lastOrderNode.Value;
                        spriteBatch.Draw(pixel, lastOrder.Position, null, Color.Green, (float)Math.Atan2(unitAction.Position.Y - lastOrder.Position.Y, unitAction.Position.X - lastOrder.Position.X), new Vector2(0, (float)pixel.Height / 2f), new Vector2((lastOrder.Position - unitAction.Position).Length(), (float)lastOrderNodes.Where(x => x.Value == lastOrder).Count() / 2), SpriteEffects.None, 0f);
                    }
                }
            }
        }
    }
}
