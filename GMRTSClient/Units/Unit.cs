using System;
using System.Collections.Generic;
using System.Text;
using GMRTSClasses.Units;
using Microsoft.Xna.Framework;

namespace GMRTSClient.Units
{
    class Unit : GMRTSClasses.Units.Unit
    {
        public LinkedList<UnitAction> Orders;
        public Vector2 CurrentPosition; //=> new Vector2(Position.Value.X, Position.Value.Y);
        public bool Selectable = true;
        public bool Selected = false;

        public Unit()
        {
            Orders = new LinkedList<UnitAction>();
        }


        public virtual Rectangle GetSelectionRect()
        {
            return Rectangle.Empty;
        }
    }
}
