using GMRTSClasses.CTSTransferData.MetaActions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.UI.ClientAction
{
    class ReplaceAction : PlayerAction
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
}
