using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator.gate
{
    public class EXOR : IGate
    {
        private EXOR() { }

        private static EXOR eXOR = new EXOR();

        public bool Execute(bool[] inputs)
        {
            throw new NotImplementedException();
        }

        public CircuitType GetCircuitType()
        {
            return CircuitType.EXOR;
        }

        public static IGate GetInstance()
        {
            return eXOR;
        }
    }
}
