using GMRTSClient.Component.Unit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMRTSClient.UI.ClientActions
{
    class DeleteAction : PlayerAction
    {
        public PlayerAction ActionToDelete;
        public Unit[] Units;
        public DeleteAction(Unit[] units, PlayerAction actionToDelete)
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
}
