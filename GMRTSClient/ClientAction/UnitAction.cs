using GMRTSClasses.CTSTransferData;
using GMRTSClasses.CTSTransferData.MetaActions;
using GMRTSClient.Components.Unit;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMRTSClient.UI.ClientActions
{
    abstract class UnitAction : PlayerAction
    {
        public List<Unit> Units;
        public Vector2 Position;

        private HashSet<UnitAction> prevOrders;
        private HashSet<Unit> currentUnits;

        private TimeSpan animationTime;

        private float scale = 0.01f;

        public UnitAction(List<Unit> units, Texture2D pixel, Texture2D circle)
            : this(Guid.NewGuid(), units, pixel, circle) { }
        public UnitAction(Guid id, List<Unit> units, Texture2D pixel, Texture2D circle)
        {
            animationTime = new TimeSpan(0, 0, 0, 0, 500);
            ID = id;
            Units = new List<Unit>(units.ToArray());
            currentUnits = new HashSet<Unit>();

            foreach (var unit in units)
            {
                unit.Orders.AddLast(this);
            }
        }
    }
}
