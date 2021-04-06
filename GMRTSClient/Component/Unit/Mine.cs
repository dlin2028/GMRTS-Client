using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended.Sprites;

using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Component.Unit
{
    class Supermarket : UnitComponent
    {
        public Supermarket(Unit unit, ContentManager content)
            : base(unit, new Sprite(content.Load<Texture2D>("Market")))
        {

        }
    }
}
