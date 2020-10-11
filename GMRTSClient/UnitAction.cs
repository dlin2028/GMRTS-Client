using GMRTSClient.Units;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient
{
    public enum ActionType
    {
        None,
        Move,
        Attack,
        Assist,
        Patrol
    }
    abstract class UnitAction
    {
        public Vector2 Position;
        public Guid TargetUnit;
        public ActionType ActionType;
        List<Unit> Units;

        public UnitAction(List<Unit> units)
        {
            Units = units;
        }
    }

    class PatrolAction : UnitAction
    {
        public PatrolAction(Vector2 position, List<Unit> units)
            :base(units)
        {
            ActionType = ActionType.Patrol;
            Position = position;
        }
    }

    class AttackAction : UnitAction
    {
        bool GroundFire;

        public AttackAction(Guid targetUnit, List<Unit> units)
            : base(units)
        {
            ActionType = ActionType.Attack;
            TargetUnit = targetUnit;
            GroundFire = false;
        }

        public AttackAction(Vector2 position, List<Unit> units)
            : base(units)
        {
            Position = position;
            GroundFire = true;
        }
    }

    class MoveAction : UnitAction
    {
        public MoveAction(Vector2 position, List<Unit> units)
            : base(units)
        {
            ActionType = ActionType.Move;
            Position = position;
        }
    }

    class AssistAction : UnitAction
    {
        public AssistAction(Guid targetUnit, List<Unit> units)
            : base(units)
        {
            ActionType = ActionType.Assist;
            TargetUnit = targetUnit;
        }
    }
}
