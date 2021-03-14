using GMRTSClient.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.UI.ClientActions
{
    abstract class UnitUnitAction : UnitAction
    {
        public Unit Target;

        public UnitUnitAction(List<Unit> units, Texture2D pixel, Unit target, Texture2D circle)
            : base(units, pixel, circle)
        {
            Target = target;
        }
    }
}
