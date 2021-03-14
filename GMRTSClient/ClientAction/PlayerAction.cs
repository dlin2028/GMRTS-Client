using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.UI.ClientActions
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

    abstract class PlayerAction
    {
        public ActionType ActionType;
        public Guid ID;

        public PlayerAction()
        {
            ID = Guid.NewGuid();
        }

        // Bad. Pls delete. Probably should be replaced by refiguring out this class structure.
        public abstract GMRTSClasses.CTSTransferData.ClientAction ToDTONonmetaAction();
        public abstract GMRTSClasses.CTSTransferData.MetaActions.MetaAction ToDTOMetaAction();
    }
}
