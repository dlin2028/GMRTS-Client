using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Units
{
    class Builder : Unit
    {
        public Builder(ContentManager content)
            :base(new Sprite(content.Load<Texture2D>("Builder")), new Sprite(content.Load<Texture2D>("SelectMarker")))
        {
            UnitType = UnitType.Builder;
        }
    }
}
