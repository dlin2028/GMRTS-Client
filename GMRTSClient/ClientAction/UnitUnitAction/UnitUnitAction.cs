using GMRTSClient.Component.Unit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.UI.ClientAction
{
    abstract class UnitUnitAction : UnitAction
    {
        public Unit Target { get; set; }
        public bool PauseUpdate { get; set; }
        public UnitUnitAction(List<Unit> units, Unit target)
            : base(units)
        {
            Target = target;
            Position = new Vector2(target.Position.Value.X, target.Position.Value.Y);
            PauseUpdate = false;
        }

        public void Update()
        {
            if(!PauseUpdate)
            {
                Position = new Vector2(Target.Position.Value.X, Target.Position.Value.Y);
            }
        }
    }
}
