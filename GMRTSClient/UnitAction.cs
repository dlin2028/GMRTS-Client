using GMRTSClient.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMRTSClient
{
    public enum ActionType
    {
        None,
        Move,
        Attack,
        Assist,
        Patrol,
        Delete
    }

    abstract class UnitAction
    {
        public ActionType ActionType;
        public List<Unit> Units;
        public Vector2 Position;
        public Guid ID;

        protected Texture2D pixel;

        private HashSet<UnitAction> prevOrders;
        private HashSet<Unit> currentUnits;

        public UnitAction(List<Unit> units, Texture2D pixel)
            : this(Guid.NewGuid(), units, pixel) { }
        public UnitAction(Guid id, List<Unit> units, Texture2D pixel)
        {
            ID = id;
            Units = units;
            this.pixel = pixel;
            currentUnits = new HashSet<Unit>();

            foreach (var unit in units)
            {
                unit.Orders.AddLast(this);
            }
        }

        public virtual void Update()
        {
            prevOrders = new HashSet<UnitAction>();
            foreach (var unit in Units)
            {
                var prevOrder = unit.Orders.Find(this).Previous;
                if (prevOrder != null)
                    prevOrders.Add(prevOrder.Value);
                else
                    currentUnits.Add(unit);
            }
        }

        public virtual void Draw(SpriteBatch sb)
            => draw(sb, Color.White);

        protected void draw(SpriteBatch sb, Color color)
        {
            if(currentUnits.Count != 0)
            {
                var avgPosition = new Vector2(currentUnits.Average(x => x.CurrentPosition.X), currentUnits.Average(x => x.CurrentPosition.Y));
                sb.Draw(pixel, new Rectangle((int)avgPosition.X, (int)avgPosition.Y, (int)(avgPosition - Position).Length(), (int)currentUnits.Count), null, color, (float)Math.Atan2(Position.Y - avgPosition.Y, Position.X - avgPosition.X), Vector2.Zero, SpriteEffects.None, 0f);
            }
            foreach (var order in prevOrders)
            {
                sb.Draw(pixel, new Rectangle((int)order.Position.X, (int)order.Position.Y, (int)(order.Position - Position).Length(), order.Units.Where(x => { var y = x.Orders.Find(order).Next; return y != null && y.Value == this; }).Count()), null, color, (float)Math.Atan2(Position.Y - order.Position.Y, Position.X - order.Position.X), Vector2.Zero, SpriteEffects.None, 0f);
            }
        }
    }

    class DeleteAction : UnitAction
    {
        public DeleteAction(List<Unit> units, Texture2D pixel) : base(units, pixel)
        {
        }
    }

    abstract class UnitGroundAction : UnitAction
    {
        public Vector2 Target;
        
        public UnitGroundAction(List<Unit> units, Texture2D pixel, Vector2 target)
            :base(units, pixel)
        {
            Position = target;
            Target = target;
        }
    }

    abstract class UnitUnitAction : UnitAction
    {
        public Unit Target;
        
        public UnitUnitAction(List<Unit> units, Texture2D pixel, Unit target)
            : base(units, pixel)
        {
            Target = target;
        }

        public override void Update()
        {
            Position = Target.CurrentPosition;
            base.Update();
        }
    }

    class PatrolAction : UnitGroundAction
    {
        List<UnitAction> lastPatrols;

        public PatrolAction(List<Unit> units, Texture2D pixel, Vector2 target)
            : base(units, pixel, target)
        {
            lastPatrols = new List<UnitAction>();
        }

        public override void Update()
        {
            foreach (var unit in Units)
            {
                lastPatrols.Add(unit.Orders.Where(x => x is PatrolAction).Last());
            }
            base.Update();
        }
        public override void Draw(SpriteBatch sb)
        {

            foreach (var order in lastPatrols)
            {
                sb.Draw(pixel, new Rectangle((int)order.Position.X, (int)order.Position.Y, (int)(order.Position - Position).Length(), (int)Math.Sqrt(Math.Min(Units.Count, order.Units.Count))), null, Color.Green, (int)Math.Atan2(Position.Y - order.Position.Y, Position.X - order.Position.X), Vector2.Zero, SpriteEffects.None, 0f);
            }

            draw(sb, Color.Green);
        }
    }

    class AttackAction : UnitUnitAction
    {
        public AttackAction(List<Unit> units, Texture2D pixel, Unit target)
            : base(units, pixel, target)
        {
        }
        public override void Draw(SpriteBatch sb)
            => draw(sb, Color.Red);
    }

    class MoveAction : UnitGroundAction
    {
        public MoveAction(List<Unit> units, Texture2D pixel, Vector2 target)
            : base(units, pixel, target)
        {
        }
        public override void Draw(SpriteBatch sb)
            => draw(sb, Color.Blue);
    }

    class AssistAction : UnitUnitAction
    {
        public AssistAction(List<Unit> units, Texture2D pixel, Unit target)
            : base(units, pixel, target)
        {
        }
        public override void Draw(SpriteBatch sb)
            => draw(sb, Color.Yellow);
    }
}
