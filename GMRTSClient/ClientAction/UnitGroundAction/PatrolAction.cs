using GMRTSClasses.CTSTransferData;
using GMRTSClasses.CTSTransferData.FactoryActions;
using GMRTSClasses.CTSTransferData.MetaActions;
using GMRTSClient.Component.Unit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMRTSClient.UI.ClientAction
{
    class PatrolAction : UnitGroundAction
    {
        List<LinkedListNode<UnitAction>> lastPatrols;

        public PatrolAction(List<Unit> units, Vector2 target)
            : base(units, target)
        {
            RenderColor = Color.Green;
            ActionType = ActionType.Patrol;
            lastPatrols = new List<LinkedListNode<UnitAction>>();
        }

        public override GMRTSClasses.CTSTransferData.ClientAction ToDTONonmetaAction()
        {
            return new GMRTSClasses.CTSTransferData.UnitGround.MoveAction() { ActionID = ID, Position = new System.Numerics.Vector2(Position.X, Position.Y), UnitIDs = Units.Select(x => x.ID).ToList(), RequeueOnCompletion = true };
        }

        public override MetaAction ToDTOMetaAction()
        {
            return null;
        }

        public override FactoryAction ToDTOFactoryAction()
        {
            return null;
        }
    }
}
