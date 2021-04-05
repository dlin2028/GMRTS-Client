using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Component.Unit
{
    abstract class UnitComponent
    {
        public Sprite Sprite;
        public Unit Unit;

        public UnitComponent(Unit unit, Sprite sprite)
        {
            Sprite = sprite;
            Unit = unit;
            unit.Sprite = sprite;
        }
    }
}
