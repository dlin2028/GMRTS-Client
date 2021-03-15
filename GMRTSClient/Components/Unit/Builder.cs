using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Components.Unit
{
    /// <summary>
    /// Builds factories and more?
    /// </summary>
    class Builder : Unit
    {
        public Builder(Guid id, ContentManager content)
            :base(id, new Sprite(content.Load<Texture2D>("Builder")))
        {
            UnitType = UnitType.Builder;
        }
    }
}
