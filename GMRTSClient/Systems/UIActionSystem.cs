using GMRTSClient.UI;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Systems
{
    class UIActionSystem : EntitySystem
    {
        private GameUI gameUI;
        public UIActionSystem(GameUI gameUI)
            :base(Aspect.All())
        {
            this.gameUI = gameUI;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            throw new NotImplementedException();
        }
    }
}
