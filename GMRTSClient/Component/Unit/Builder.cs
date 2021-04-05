using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Component.Unit
{
    /// <summary>
    /// Builds factories and more?
    /// </summary>
    class Builder : UnitComponent
    {
        public Builder(Unit unit, ContentManager content)
            :base(unit, new Sprite(content.Load<Texture2D>("Builder")))
        {

        }
    }
}
