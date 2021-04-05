using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GMRTSClient.UI.ClientAction
{
    public enum ActionType
    {
        None,
        Move,
        Attack,
        Assist,
        Patrol,
        Build,
        //non drawable actions
        Factory,
        Delete,
        Replace
    }

    abstract class PlayerAction
    {
        public ActionType ActionType;
        public Guid ID;
        public bool IsUnitAction => ActionType <= ActionType.Build && ActionType != ActionType.None;

        public PlayerAction()
        {
            ID = Guid.NewGuid();
        }
        // Bad. Pls delete. Probably should be replaced by refiguring out this class structure.
        public abstract GMRTSClasses.CTSTransferData.ClientAction ToDTONonmetaAction();
        public abstract GMRTSClasses.CTSTransferData.MetaActions.MetaAction ToDTOMetaAction();
        public abstract GMRTSClasses.CTSTransferData.FactoryActions.FactoryAction ToDTOFactoryAction();
    }
}
