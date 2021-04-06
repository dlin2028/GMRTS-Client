using GMRTSClient.UI.ClientAction;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Component
{
    class DTOActionData
    {
        public PlayerAction Action;

        public GMRTSClasses.CTSTransferData.ClientAction DTONonmetaAction => action.ToDTONonmetaAction();
        public GMRTSClasses.CTSTransferData.MetaActions.MetaAction DTOMetaAction => action.ToDTOMetaAction();
        public GMRTSClasses.CTSTransferData.FactoryActions.FactoryAction DTOFactoryOrder => action.ToDTOFactoryAction();

        public DTOActionData(PlayerAction action)
        {
            Action = action;
        }
    }
}
