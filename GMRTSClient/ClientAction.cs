using GMRTSClient.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        Delete,
        Replace
    }

    abstract class ClientAction
    {
        public ActionType ActionType;
        public Guid ID;

        public ClientAction()
        {
            ID = Guid.NewGuid();
        }
    }

    class DeleteAction : ClientAction
    {
        public Guid ActionToDelete;
        public DeleteAction(Guid actionToDelete)
        {
            ActionToDelete = actionToDelete;
            ActionType = ActionType.Delete;
        }
    }

    class ReplaceAction : ClientAction
    {
        public Guid OldId;
        public UnitAction NewAction;

        public ReplaceAction(UnitAction replacedAction)
        {
            NewAction = replacedAction;
            OldId = replacedAction.ID;
            replacedAction.ID = Guid.NewGuid();
            ActionType = ActionType.Replace;
        }
    }

    abstract class UnitAction : ClientAction
    {
        public List<Unit> Units;
        public Vector2 Position;

        protected Texture2D pixel;
        protected Texture2D circle;

        private HashSet<UnitAction> prevOrders;
        private HashSet<Unit> currentUnits;

        private TimeSpan animationTime;

        private float scale = 0.01f;

        public UnitAction(List<Unit> units, Texture2D pixel, Texture2D circle)
            : this(Guid.NewGuid(), units, pixel, circle) { }
        public UnitAction(Guid id, List<Unit> units, Texture2D pixel, Texture2D circle)
        {
            animationTime = new TimeSpan(0,0,0,0,500);
            ID = id;
            Units = new List<Unit>(units.ToArray());
            this.pixel = pixel;
            this.circle = circle;
            currentUnits = new HashSet<Unit>();
            prevOrders = new HashSet<UnitAction>();

            foreach (var unit in units)
            {
                unit.Orders.AddLast(this);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            animationTime -= gameTime.ElapsedGameTime;

            prevOrders.Clear();
            currentUnits.Clear();
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

        public bool Intersecting(Vector2 other)
        {
            return (other - Position).Length() < circle.Width * scale;
        }

        protected void draw(SpriteBatch sb, Color color)
        {
            if(animationTime.TotalMilliseconds > 0)
            {
                sb.Draw(circle, Position, null, color * (float)(animationTime.TotalMilliseconds / 500.0), 0f, new Vector2(circle.Width, circle.Height)/2, 0.01f, SpriteEffects.None, 0f);
            }

            if(InputManager.Keys.IsKeyDown(Keys.LeftShift))
            {
                sb.Draw(circle, Position, null, color, 0f, new Vector2(circle.Width, circle.Height) / 2, scale, SpriteEffects.None, 0f);
                if (currentUnits.Count != 0)
                {
                    var avgPosition = new Vector2(currentUnits.Average(x => x.CurrentPosition.X), currentUnits.Average(x => x.CurrentPosition.Y));
                    sb.Draw(pixel, avgPosition, null, color, (float)Math.Atan2(Position.Y - avgPosition.Y, Position.X - avgPosition.X), new Vector2(0, (float)pixel.Height/2f), new Vector2((avgPosition - Position).Length(), (float)currentUnits.Count / 2), SpriteEffects.None, 0f);
                }
                foreach (var order in prevOrders)
                {
                    sb.Draw(pixel, order.Position, null, color, (float)Math.Atan2(Position.Y - order.Position.Y, Position.X - order.Position.X), new Vector2(0, (float)pixel.Height/2f), new Vector2((order.Position - Position).Length(), (float)order.Units.Where(x => { var y = x.Orders.Find(order).Next; return y != null && y.Value == this; }).Count()/2), SpriteEffects.None, 0f);
                }
            }
        }
    }

    abstract class UnitGroundAction : UnitAction
    {
        public Vector2 Target;
        
        public UnitGroundAction(List<Unit> units, Texture2D pixel, Vector2 target, Texture2D circle)
            :base(units, pixel, circle)
        {
            Position = target;
            Target = target;
        }
    }

    abstract class UnitUnitAction : UnitAction
    {
        public Unit Target;
        
        public UnitUnitAction(List<Unit> units, Texture2D pixel, Unit target, Texture2D circle)
            : base(units, pixel, circle)
        {
            Target = target;
        }

        public override void Update(GameTime gameTime)
        {
            Position = Target.CurrentPosition;
            base.Update(gameTime);
        }
    }

    class PatrolAction : UnitGroundAction
    {
        List<LinkedListNode<UnitAction>> lastPatrols;

        public PatrolAction(List<Unit> units, Texture2D pixel, Vector2 target, Texture2D circle)
            : base(units, pixel, target, circle)
        {
            ActionType = ActionType.Patrol;
            lastPatrols = new List<LinkedListNode<UnitAction>>();
        }

        public override void Update(GameTime gameTime)
        {
            lastPatrols.Clear();
            foreach (var unit in Units)
            {
                LinkedListNode<UnitAction> curr = unit.Orders.First;
                LinkedListNode<UnitAction> last = null;
                while(curr != null)
                {
                    if(curr.Value.ActionType == ActionType.Patrol)
                    {
                        last = curr;
                    }

                    curr = curr.Next;
                }
                if(last != null)
                {
                    lastPatrols.Add(last);
                }
            }
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch sb)
        {
            var groups = lastPatrols.GroupBy(x => x);
            foreach (var orderGroup in groups)
            {
                if(orderGroup.Key.List.First(x => x.ActionType == ActionType.Patrol).ID == ID)
                {
                    var order = orderGroup.Key.Value;
                    sb.Draw(pixel, order.Position, null, Color.Green, (float)Math.Atan2(Position.Y - order.Position.Y, Position.X - order.Position.X), new Vector2(0, (float)pixel.Height/2f), new Vector2((order.Position - Position).Length(), orderGroup.Count()/2), SpriteEffects.None, 0f);
                }
            }

            draw(sb, Color.Green);
        }
    }

    class AttackAction : UnitUnitAction
    {
        public AttackAction(List<Unit> units, Texture2D pixel, Unit target, Texture2D circle) : base(units, pixel, target, circle)
        {
            ActionType = ActionType.Attack;
        }

        public override void Draw(SpriteBatch sb)
            => draw(sb, Color.Red);
    }

    class MoveAction : UnitGroundAction
    {
        public MoveAction(List<Unit> units, Texture2D pixel, Vector2 target, Texture2D circle) : base(units, pixel, target, circle)
        {
            ActionType = ActionType.Move;
        }

        public override void Draw(SpriteBatch sb)
            => draw(sb, Color.Blue);
    }

    class AssistAction : UnitUnitAction
    {
        public AssistAction(List<Unit> units, Texture2D pixel, Unit target, Texture2D circle) : base(units, pixel, target, circle)
        {
            ActionType = ActionType.Assist;
        }

        public override void Draw(SpriteBatch sb)
            => draw(sb, Color.Yellow);
    }
}
