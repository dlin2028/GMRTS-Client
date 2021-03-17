using GMRTSClient.Component.Unit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.UI.ClientAction
{
    abstract class UnitGroundAction : UnitAction
    {
        public Vector2 Target;

        public UnitGroundAction(List<Unit> units, Vector2 target)
            : base(units)
        {
            Position = target;
            Target = target;
        }
    }
}
