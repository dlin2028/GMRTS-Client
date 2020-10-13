﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMRTSServer.UnitStates
{
    internal interface IUnitOrder
    {
        ContOrStop Update(ulong currentMilliseconds, float elapsedTime);

        Guid ID { get; set; }
    }
}
