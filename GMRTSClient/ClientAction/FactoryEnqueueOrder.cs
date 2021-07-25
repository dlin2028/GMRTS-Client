using GMRTSClasses.CTSTransferData.FactoryActions;
using GMRTSClasses.Units;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.ClientAction
{
    class FactoryEnqueueOrder : FactoryOrder
    {
        public MobileUnitType UnitType {get; set;}
        public FactoryEnqueueOrder(Guid targetFactory, MobileUnitType unitType)
            : base(targetFactory)
        {
            UnitType = unitType;
        }


        public override FactoryAction ToDTOFactoryAction()
        {
            return new GMRTSClasses.CTSTransferData.FactoryActions.EnqueueBuildOrder() { TargetFactory = targetFactory, OrderID = ID, UnitType = UnitType };
        }
    }
}
