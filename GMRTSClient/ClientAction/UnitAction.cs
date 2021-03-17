using GMRTSClasses.CTSTransferData;
using GMRTSClasses.CTSTransferData.MetaActions;
using GMRTSClient.Component.Unit;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMRTSClient.UI.ClientAction
{
    abstract class UnitAction : PlayerAction
    {
        public ObservableCollection<Unit> Units { get; set; }
        public Vector2 Position { get; set; }
        public Color RenderColor { get; protected set; }

        public HashSet<UnitAction> PrevOrders { get; private set; }

        public HashSet<Unit> CurrentUnits { get; private set; }

        private TimeSpan animationTime;

        private float scale = 0.01f;

        public UnitAction(List<Unit> units)
            : this(Guid.NewGuid(), units) { }
        public UnitAction(Guid id, List<Unit> units)
        {
            animationTime = new TimeSpan(0, 0, 0, 0, 500);
            ID = id;
            Units = new ObservableCollection<Unit>(units);
            PrevOrders = new HashSet<UnitAction>();
            CurrentUnits = new HashSet<Unit>();

            Units.Clearing += updateCollections;
            Units.ItemAdded += updateCollections;
            Units.ItemRemoved += updateCollections;

            foreach (var unit in units)
            {
                unit.Orders.AddLast(this);
            }
            updateCollections(null, null);
        }

        private void updateCollections(object sender, EventArgs e)
        {
            PrevOrders.Clear();
            CurrentUnits.Clear();
            foreach (var unit in Units)
            {
                var prevOrder = unit.Orders.Find(this).Previous;
                if (prevOrder != null)
                    PrevOrders.Add(prevOrder.Value);
                else
                    CurrentUnits.Add(unit);
            }
        }
    }
}
