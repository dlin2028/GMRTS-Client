using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Units
{
    class Builder : Unit
    {
        public Builder(ContentManager content)
        {
            Texture = content.Load<Texture2D>("Tank");
            UnitType = UnitType.Builder;
        }
    }
}
