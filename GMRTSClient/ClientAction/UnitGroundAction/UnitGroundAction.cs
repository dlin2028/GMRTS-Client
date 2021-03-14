using GMRTSClient.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.UI.ClientActions
{
    abstract class UnitGroundAction : UnitAction
    {
        public Vector2 Target;

        public UnitGroundAction(List<Unit> units, Texture2D pixel, Vector2 target, Texture2D circle)
            : base(units, pixel, circle)
        {
            Position = target;
            Target = target;
        }
    }
}
