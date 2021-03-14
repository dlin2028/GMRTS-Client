using GMRTSClasses.CTSTransferData;
using GMRTSClasses.CTSTransferData.MetaActions;
using GMRTSClient.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMRTSClient.UI.ClientActions
{
    class PatrolAction : UnitGroundAction
    {
        List<LinkedListNode<UnitAction>> lastPatrols;

        public PatrolAction(List<Unit> units, Texture2D pixel, Vector2 target, Texture2D circle)
            : base(units, pixel, target, circle)
        {
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
    }
}
