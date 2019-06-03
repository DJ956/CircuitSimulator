using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator.gate
{
    public class EXOR : IGate
    {
        private EXOR() { }

        private static EXOR eXOR = new EXOR();

        /// <summary>
        /// 真の数が奇数個ならTrue,偶数個ならFalse
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public bool Execute(bool[] inputs)
        {
            var count = 0;
            foreach(var b in inputs)
            {
                if (b) { count++; }
            }
            if(count % 2 == 0) { return false; }
            return true;
        }

        public CircuitType GetCircuitType() { return CircuitType.EXOR; }

        public static IGate GetInstance() { return eXOR; }

        public bool EquivalentFailure(bool[] normalInputs, bool[] faultInputs)
        {
            return Execute(normalInputs) == Execute(faultInputs);
        }
    }
}
