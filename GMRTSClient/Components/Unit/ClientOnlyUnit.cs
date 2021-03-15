using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Components.Unit
{
    /// <summary>
    /// Units which don't rely on the server for transform data
    /// </summary>
    class ClientOnlyUnit : Unit
    {
        public ClientOnlyUnit(ContentManager content)
            : base(Guid.Empty, new Sprite(content.Load<Texture2D>("Builder")), new Sprite(content.Load<Texture2D>("SelectMarker")))
        {

        }
    }
}
