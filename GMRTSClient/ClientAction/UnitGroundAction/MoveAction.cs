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
    class MoveAction : UnitGroundAction
    {
        public MoveAction(List<Unit> units, Vector2 target) : base(units, target)
        {
            RenderColor = Color.Blue;
            ActionType = ActionType.Move;
        }

        public override MetaAction ToDTOMetaAction()
        {
            return null;
        }

        public override GMRTSClasses.CTSTransferData.ClientAction ToDTONonmetaAction()
        {
            return new GMRTSClasses.CTSTransferData.UnitGround.MoveAction() { ActionID = ID, Position = new System.Numerics.Vector2(Position.X, Position.Y), UnitIDs = Units.Select(x => x.ID).ToList(), RequeueOnCompletion = false };
        }

        public override GMRTSClasses.CTSTransferData.FactoryActions.FactoryAction ToDTOFactoryAction()
        {
            return null;
        }
    }
}
