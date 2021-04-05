using GMRTSClasses.CTSTransferData.MetaActions;
using GMRTSClasses.CTSTransferData.UnitGround;
using GMRTSClasses.Units;
using GMRTSClient.Component.Unit;
using GMRTSClient.UI.ClientAction;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.ClientAction
{
    abstract class FactoryOrder : PlayerAction
    {
        protected Guid targetFactory;
        public FactoryOrder(Guid targetFactory)
        {
            this.targetFactory = targetFactory;
        }

        public override MetaAction ToDTOMetaAction()
        {
            return null;
        }

        public override GMRTSClasses.CTSTransferData.ClientAction ToDTONonmetaAction()
        {
            return null;
        }
    }
}
