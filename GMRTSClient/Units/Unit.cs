using System;
using System.Collections.Generic;
using System.Text;
using GMRTSClasses.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GMRTSClient.Units
{
    class Unit : GMRTSClasses.Units.Unit
    {
        public LinkedList<UnitAction> Orders;
        public /*readonly*/ Vector2 CurrentPosition => new Vector2(Position.Value.X, Position.Value.Y);
        public bool Selectable = true;
        public bool Selected = false;
        public Unit(Guid id)
            :base(id)
        {
            Orders = new LinkedList<UnitAction>();
        }

        public virtual bool Intersecting(Vector2 vector)
        {
            return false;
        }
        public virtual bool Intersecting(Rectangle rect)
        {
            return false;
        }

        public virtual void Draw(SpriteBatch sb)
        {

        }
    }
}
