using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GMRTSClasses;
using GMRTSClasses.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GMRTSClient.Units
{
    class Tank : Unit
    {
        public Tank(Guid ID, ContentManager content)
            : base(ID, content.Load<Texture2D>("Tank"), content.Load<Texture2D>("SelectionMarker"))
        {
            Position = new GMRTSClasses.Changing<System.Numerics.Vector2>(System.Numerics.Vector2.Zero, System.Numerics.Vector2.Zero, Vector2Changer.VectorChanger, 0);
            Rotation = new GMRTSClasses.Changing<float>(0, 0, FloatChanger.FChanger, 0);
            Health = new GMRTSClasses.Changing<float>(100, 0, FloatChanger.FChanger, 0);
        }
    }
}
