using GMRTSClasses;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Units
{
    /// <summary>
    /// Builds factories and more?
    /// </summary>
    class Builder : Unit
    {
        public Builder(Guid id, ContentManager content)
            :base(id, content.Load<Texture2D>("Builder"), content.Load<Texture2D>("SelectionMarker"))
        {
        }
    }
}
