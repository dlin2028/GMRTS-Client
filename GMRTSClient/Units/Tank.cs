using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Units
{
    class Tank : Unit
    {
        public Tank(ContentManager content)
            : base(new Sprite(content.Load<Texture2D>("Tank")), new Sprite(content.Load<Texture2D>("SelectMarker")))
        {
            UnitType = UnitType.Tank;
        }
    }
}
