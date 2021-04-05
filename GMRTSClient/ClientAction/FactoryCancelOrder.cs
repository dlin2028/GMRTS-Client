using GMRTSClasses.CTSTransferData.FactoryActions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.ClientAction
{
    class FactoryCancelOrder : FactoryOrder
    {
        Guid targetOrder;
        public FactoryCancelOrder(Guid targetFactory, Guid targetOrder)
            :base(targetFactory)
        {
            this.targetOrder = targetOrder;
        }
        public override FactoryAction ToDTOFactoryAction()
        {
            return new GMRTSClasses.CTSTransferData.FactoryActions.CancelBuildOrder() { TargetFactory = targetFactory, OrderID = targetOrder };
        }
    }
}
