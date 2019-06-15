using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator
{
    [Serializable]
    public class CircleFault : IEquatable<CircleFault>
    {
        /// <summary>
        /// 故障するインデックス番号
        /// </summary>
        public int FaultIndex { get; set; }

        /// <summary>
        /// 故障する値
        /// </summary>
        public bool FaultValue { get; set; }

        public CircleFault(int faultIndex, bool faultValue)
        {
            FaultIndex = faultIndex;
            FaultValue = faultValue;
        }

        public CircleFault() { }

        public override int GetHashCode()
        {
            return this.FaultIndex.GetHashCode();
        }

        public bool Equals(CircleFault other)
        {
            if(other == null) { return false; }
            return (this.FaultIndex == other.FaultIndex);
        }
    }
}
