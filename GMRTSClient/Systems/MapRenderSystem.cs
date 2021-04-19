using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Systems
{
    class MapRenderSystem : EntityDrawSystem
    {
        TiledMap tiledMap;
        TiledMapRenderer tiledMapRenderer;
        private readonly OrthographicCamera camera;
        public MapRenderSystem(ContentManager content, GraphicsDevice graphics, OrthographicCamera camera)
            :base(Aspect.All())
        {
            this.camera = camera;
            tiledMap = content.Load<TiledMap>("puddles");
            tiledMapRenderer = new TiledMapRenderer(graphics, tiledMap);
        }

        public override void Draw(GameTime gameTime)
        {
            tiledMapRenderer.Draw(camera.GetViewMatrix());
        }

        public override void Initialize(IComponentMapperService mapperService)
        {

        }
    }
}
