using GMRTSClient.ClientAction;
using GMRTSClient.UI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Component.Unit
{
    class Factory : UnitComponent
    {
        public List<FactoryOrder> Orders;
        public BuildFlags BuildFlags = BuildFlags.Unit;
        public Factory(Unit unit, ContentManager content)
            : base(unit, new Sprite(content.Load<Texture2D>("Factory")))
        {
            Orders = new List<FactoryOrder>();
        }
    }
}
