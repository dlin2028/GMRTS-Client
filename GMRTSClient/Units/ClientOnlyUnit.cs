using GMRTSClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMRTSClient.Units
{

    /// <summary>
    /// Units which don't rely on the server for transform data
    /// </summary>
    class ClientOnlyUnit : Unit
    {
        public ClientOnlyUnit(Texture2D texture, Texture2D selectionTexture, Vector2 position, float rotation)
            : base(Guid.Empty, texture, selectionTexture)
        {
            Position = new GMRTSClasses.Changing<System.Numerics.Vector2>(new System.Numerics.Vector2(position.X, position.Y), System.Numerics.Vector2.Zero, Vector2Changer.VectorChanger, 0);
            Rotation = new GMRTSClasses.Changing<float>(rotation, 5, FloatChanger.FChanger, 0);
        }
    }
}
