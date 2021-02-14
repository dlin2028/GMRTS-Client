using GMRTSClasses.CTSTransferData;
using GMRTSClasses.CTSTransferData.MetaActions;
using GMRTSClient.UI.Display;
using GMRTSClient.Units;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMRTSClient.UI
{
    public enum ActionType
    {
        None,
        Move,
        Attack,
        Assist,
        Patrol,
        Delete,
        Replace,
        Build
    }

    abstract class ClientAction
    {
        public ActionType ActionType;
        public Guid ID;

        public ClientAction()
        {
            ID = Guid.NewGuid();
        }

        // Bad. Pls delete. Probably should be replaced by refiguring out this class structure.
        public abstract GMRTSClasses.CTSTransferData.ClientAction ToDTONonmetaAction();
        public abstract GMRTSClasses.CTSTransferData.MetaActions.MetaAction ToDTOMetaAction();
    }

    class DeleteAction : ClientAction
    {
        public ClientAction ActionToDelete;
        public Unit[] Units;
        public DeleteAction(Unit[] units, ClientAction actionToDelete)
        {
            ActionToDelete = actionToDelete;
            ActionType = ActionType.Delete;
            Units = units;
        }

        public override GMRTSClasses.CTSTransferData.MetaActions.MetaAction ToDTOMetaAction()
        {
            return new GMRTSClasses.CTSTransferData.MetaActions.DeleteAction() { AffectedUnits = Units.Select(a => a.ID).ToList(), TargetActionID = ActionToDelete.ID };
        }

        public override GMRTSClasses.CTSTransferData.ClientAction ToDTONonmetaAction()
        {
            return null;
        }
    }

    class ReplaceAction : ClientAction
    {
        public Guid OldId;
        public UnitAction NewAction;

        public ReplaceAction(UnitAction newAction, Guid oldID)
        {
            NewAction = newAction;
            OldId = oldID;
            newAction.ID = Guid.NewGuid();
            ActionType = ActionType.Replace;
        }

        public override MetaAction ToDTOMetaAction()
        {
            var replacementAction = NewAction.ToDTONonmetaAction();
            return replacementAction switch
            {
                GMRTSClasses.CTSTransferData.UnitGround.MoveAction mv => new GMRTSClasses.CTSTransferData.MetaActions.ReplaceAction<GMRTSClasses.CTSTransferData.UnitGround.MoveAction>() { AffectedUnits = replacementAction.UnitIDs, NewAction = mv, TargetActionID = OldId },
                GMRTSClasses.CTSTransferData.UnitGround.BuildBuildingAction bb => new GMRTSClasses.CTSTransferData.MetaActions.ReplaceAction<GMRTSClasses.CTSTransferData.UnitGround.BuildBuildingAction>() { AffectedUnits = replacementAction.UnitIDs, NewAction = bb, TargetActionID = OldId },
                GMRTSClasses.CTSTransferData.UnitUnit.AttackAction at => new GMRTSClasses.CTSTransferData.MetaActions.ReplaceAction<GMRTSClasses.CTSTransferData.UnitUnit.AttackAction>() { AffectedUnits = replacementAction.UnitIDs, NewAction = at, TargetActionID = OldId },
                GMRTSClasses.CTSTransferData.UnitUnit.AssistAction assist => new GMRTSClasses.CTSTransferData.MetaActions.ReplaceAction<GMRTSClasses.CTSTransferData.UnitUnit.AssistAction>() { AffectedUnits = replacementAction.UnitIDs, NewAction = assist, TargetActionID = OldId },
                _ => throw new Exception()
            };
        }

        public override GMRTSClasses.CTSTransferData.ClientAction ToDTONonmetaAction()
        {
            return null;
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
            animationTime = new TimeSpan(0, 0, 0, 0, 500);
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
            if (animationTime.TotalMilliseconds > 0)
            {
                sb.Draw(circle, Position, null, color * (float)(animationTime.TotalMilliseconds / 500.0), 0f, new Vector2(circle.Width, circle.Height) / 2, 0.01f, SpriteEffects.None, 0f);
            }

            if (InputManager.Keys.IsKeyDown(Keys.LeftShift))
            {
                sb.Draw(circle, Position, null, color, 0f, new Vector2(circle.Width, circle.Height) / 2, scale, SpriteEffects.None, 0f);
                if (currentUnits.Count != 0)
                {
                    var avgPosition = new Vector2(currentUnits.Average(x => x.CurrentPosition.X), currentUnits.Average(x => x.CurrentPosition.Y));
                    sb.Draw(pixel, avgPosition, null, color, (float)Math.Atan2(Position.Y - avgPosition.Y, Position.X - avgPosition.X), new Vector2(0, (float)pixel.Height / 2f), new Vector2((avgPosition - Position).Length(), (float)currentUnits.Count / 2), SpriteEffects.None, 0f);
                }
                foreach (var order in prevOrders)
                {
                    sb.Draw(pixel, order.Position, null, color, (float)Math.Atan2(Position.Y - order.Position.Y, Position.X - order.Position.X), new Vector2(0, (float)pixel.Height / 2f), new Vector2((order.Position - Position).Length(), (float)order.Units.Where(x => { var y = x.Orders.Find(order).Next; return y != null && y.Value == this; }).Count() / 2), SpriteEffects.None, 0f);
                }
            }
        }
    }

    abstract class UnitGroundAction : UnitAction
    {
        public Vector2 Target;

        public UnitGroundAction(List<Unit> units, Texture2D pixel, Vector2 target, Texture2D circle)
            : base(units, pixel, circle)
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

    class BuildAction : UnitGroundAction
    {
        public BuildingType BuildingType;

        BuildPreviewElement buildPreview;
        public BuildAction(List<Unit> units, Texture2D pixel, Vector2 target, BuildingType buildingType, Texture2D circle, ContentManager content) : base(units, pixel, target, circle)
        {
            ActionType = ActionType.Build;
            BuildingType = buildingType;
            buildPreview = new BuildPreviewElement(content.Load<Texture2D>("Factory"), content.Load<Texture2D>("Mine"), content.Load<Texture2D>("Market"), 0.25f);
            buildPreview.CurrentBuilding = buildingType;
            buildPreview.Location = target.ToPoint() - (buildPreview.Rect.Size.ToVector2() / 2).ToPoint();
        }

        public override void Draw(SpriteBatch sb)
        {
            buildPreview.Draw(sb);
            draw(sb, Color.Gray);
        }

        public override MetaAction ToDTOMetaAction()
        {
            return null;
        }

        public override GMRTSClasses.CTSTransferData.ClientAction ToDTONonmetaAction()
        {
            return new GMRTSClasses.CTSTransferData.UnitGround.BuildBuildingAction() { ActionID = ID, BuildingType = BuildingType, Position = new System.Numerics.Vector2(Position.X, Position.Y), UnitIDs = new List<Guid>() { Units.First().ID } };
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
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch sb)
        {
            if (InputManager.Keys.IsKeyDown(Keys.LeftShift))
            {
                var lastOrderNodes = Units.Select(x => x.Orders).Select(x => x.Last).Distinct();
                foreach (var lastOrderNode in lastOrderNodes)
                {
                    if (lastOrderNode.List.First(x => x.ActionType == ActionType.Patrol).ID == ID)
                    {
                        var lastOrder = lastOrderNode.Value;
                        sb.Draw(pixel, lastOrder.Position, null, Color.Green, (float)Math.Atan2(Position.Y - lastOrder.Position.Y, Position.X - lastOrder.Position.X), new Vector2(0, (float)pixel.Height / 2f), new Vector2((lastOrder.Position - Position).Length(), (float)lastOrderNodes.Where(x => x.Value == lastOrder).Count() / 2), SpriteEffects.None, 0f);
                    }
                }
            }

            draw(sb, Color.Green);
        }

        public override GMRTSClasses.CTSTransferData.ClientAction ToDTONonmetaAction()
        {
            return new GMRTSClasses.CTSTransferData.UnitGround.MoveAction() { ActionID = ID, Position = new System.Numerics.Vector2(Position.X, Position.Y), UnitIDs = Units.Select(x => x.ID).ToList(), RequeueOnCompletion = true };
        }

        public override MetaAction ToDTOMetaAction()
        {
            return null;
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

        public override MetaAction ToDTOMetaAction()
        {
            return null;
        }

        public override GMRTSClasses.CTSTransferData.ClientAction ToDTONonmetaAction()
        {
            return new GMRTSClasses.CTSTransferData.UnitUnit.AttackAction() { ActionID = ID, Target = Target.ID, UnitIDs = Units.Select(x => x.ID).ToList() };
        }
    }

    class MoveAction : UnitGroundAction
    {
        public MoveAction(List<Unit> units, Texture2D pixel, Vector2 target, Texture2D circle) : base(units, pixel, target, circle)
        {
            ActionType = ActionType.Move;
        }

        public override void Draw(SpriteBatch sb)
            => draw(sb, Color.Blue);

        public override MetaAction ToDTOMetaAction()
        {
            return null;
        }

        public override GMRTSClasses.CTSTransferData.ClientAction ToDTONonmetaAction()
        {
            return new GMRTSClasses.CTSTransferData.UnitGround.MoveAction() { ActionID = ID, Position = new System.Numerics.Vector2(Position.X, Position.Y), UnitIDs = Units.Select(x => x.ID).ToList(), RequeueOnCompletion = false };
        }
    }

    class AssistAction : UnitUnitAction
    {
        public AssistAction(List<Unit> units, Texture2D pixel, Unit target, Texture2D circle) : base(units, pixel, target, circle)
        {
            ActionType = ActionType.Assist;
        }

        public override void Draw(SpriteBatch sb)
            => draw(sb, Color.Yellow);

        public override MetaAction ToDTOMetaAction()
        {
            return null;
        }

        public override GMRTSClasses.CTSTransferData.ClientAction ToDTONonmetaAction()
        {
            return new GMRTSClasses.CTSTransferData.UnitUnit.AssistAction() { ActionID = ID, Target = Target.ID, UnitIDs = Units.Select(x => x.ID).ToList() };
        }
    }
}
