using GMRTSClasses;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Component.Unit
{
    /// <summary>
    /// Units which don't rely on the server for transform data
    /// </summary>
    class ClientOnlyUnit : UnitComponent
    {
        public ClientOnlyUnit(Unit unit, Transform2 transform, UnitComponent unitComponent, int health)
            :base(unit, unitComponent.Sprite)
        {
            unit.Position = new GMRTSClasses.Changing<System.Numerics.Vector2>(new System.Numerics.Vector2(transform.Position.X, transform.Position.Y), System.Numerics.Vector2.Zero, Vector2Changer.VectorChanger, 0);
            unit.Rotation = new GMRTSClasses.Changing<float>(transform.Rotation, 0.5f, FloatChanger.FChanger, 0);
            unit.Health = new Changing<float>(health, 0, FloatChanger.FChanger, 0);
        }
    }
}
