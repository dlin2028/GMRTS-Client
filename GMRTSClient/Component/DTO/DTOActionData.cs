using GMRTSClient.UI.ClientAction;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Component
{
    class DTOActionData
    {
        public PlayerAction Action;

        public GMRTSClasses.CTSTransferData.ClientAction DTONonmetaAction => Action.ToDTONonmetaAction();
        public GMRTSClasses.CTSTransferData.MetaActions.MetaAction DTOMetaAction => Action.ToDTOMetaAction();
        public GMRTSClasses.CTSTransferData.FactoryActions.FactoryAction DTOFactoryOrder => Action.ToDTOFactoryAction();

        public DTOActionData(PlayerAction action)
        {
            Action = action;
        }
    }
}
