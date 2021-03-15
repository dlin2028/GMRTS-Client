using GMRTSClasses.CTSTransferData.MetaActions;
using GMRTSClient.Component.Unit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMRTSClient.UI.ClientActions
{
    class AttackAction : UnitUnitAction
    {
        public AttackAction(List<Unit> units, Unit target) : base(units, target)
        {
            RenderColor = Color.Red;
            ActionType = ActionType.Attack;
        }

        public override MetaAction ToDTOMetaAction()
        {
            return null;
        }

        public override GMRTSClasses.CTSTransferData.ClientAction ToDTONonmetaAction()
        {
            return new GMRTSClasses.CTSTransferData.UnitUnit.AttackAction() { ActionID = ID, Target = Target.ID, UnitIDs = Units.Select(x => x.ID).ToList() };
        }
    }
}
