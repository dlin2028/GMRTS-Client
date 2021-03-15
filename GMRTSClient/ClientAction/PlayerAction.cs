using System;
using System.Collections.Generic;
using System.Drawing;
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
        //non drawable actions
        Delete,
        Replace,
        Build
    }

    class DTOActionData
    {
        private PlayerAction action;

        public GMRTSClasses.CTSTransferData.ClientAction DTONonmetaAction => action.ToDTONonmetaAction();
        public GMRTSClasses.CTSTransferData.MetaActions.MetaAction DTOMetaAction => action.ToDTOMetaAction();

        public DTOActionData(PlayerAction action)
        {
            this.action = action;
        }
    }

    abstract class PlayerAction
    {
        public ActionType ActionType;
        public Guid ID;
        public TimeSpan AnimationTime { get; set; }
        public bool IsUnitAction => ActionType <= ActionType.Patrol && ActionType != ActionType.None;

        public PlayerAction()
        {
            ID = Guid.NewGuid();
            AnimationTime = new TimeSpan(0, 0, 0, 0, 500);
        }
        // Bad. Pls delete. Probably should be replaced by refiguring out this class structure.
        public abstract GMRTSClasses.CTSTransferData.ClientAction ToDTONonmetaAction();
        public abstract GMRTSClasses.CTSTransferData.MetaActions.MetaAction ToDTOMetaAction();
    }
}
