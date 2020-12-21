using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Units
{
    class Builder : Tank
    {
        public Builder(Guid id, float rotation, float scale, Texture2D texture, Texture2D selectionTexture)
            :base(id, rotation, scale, texture, selectionTexture)
        {

        }
    }
}
