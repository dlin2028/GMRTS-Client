using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Units
{
    enum UnitType
    {
        Tank,
        Builder
    }
    abstract class Unit : GMRTSClasses.Units.Unit
    {
        public Texture2D Texture;
        public UnitType UnitType;
    }
}
