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
        public List<Unit> Units { get; set; }
        public Vector2 Position { get; set; }
        public Color RenderColor { get; protected set; }

        public HashSet<UnitAction> PrevOrders { get; private set; }

        private HashSet<Unit> currentUnits;

        private TimeSpan animationTime;

        private float scale = 0.01f;

        public UnitAction(List<Unit> units)
            : this(Guid.NewGuid(), units) { }
        public UnitAction(Guid id, List<Unit> units)
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

        public void Update()
        {
            PrevOrders.Clear();
            currentUnits.Clear();
            foreach (var unit in Units)
            {
                var prevOrder = unit.Orders.Find(this).Previous;
                if (prevOrder != null)
                    PrevOrders.Add(prevOrder.Value);
                else
                    currentUnits.Add(unit);
            }
        }
    }
}
