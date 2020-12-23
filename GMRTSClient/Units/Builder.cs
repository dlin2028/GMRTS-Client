using GMRTSClasses;

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
            Position = new GMRTSClasses.Changing<System.Numerics.Vector2>(System.Numerics.Vector2.Zero, System.Numerics.Vector2.Zero, Vector2Changer.VectorChanger, 0);
            Rotation = new GMRTSClasses.Changing<float>(0, 0, FloatChanger.FChanger, 0);
            Health = new GMRTSClasses.Changing<float>(100, 0, FloatChanger.FChanger, 0);
        }
    }
}
