using GMRTSClient.Component.Unit;
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

        public UnitUnitAction(List<Unit> units, Unit target)
            : base(units)
        {
            Target = target;
        }
    }
}
