using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator
{
    public class CircleFault
    {
        /// <summary>
        /// 故障するインデックス番号
        /// </summary>
        public int FaultIndex { get; private set; }

        /// <summary>
        /// 故障する値
        /// </summary>
        public bool FaultValue { get; private set; }

        public CircleFault(int faultIndex, bool faultValue)
        {
            FaultIndex = faultIndex;
            FaultValue = faultValue;
        }
    }
}
