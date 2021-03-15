using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Component.Unit
{
    /// <summary>
    /// Goes pew pew!
    /// </summary>
    class Tank : Unit
    {
        public Tank(Guid id, ContentManager content)
            : base(id, new Sprite(content.Load<Texture2D>("Tank")))
        {
            UnitType = UnitType.Tank;
        }
    }
}
