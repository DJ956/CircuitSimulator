using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator.gate
{
    public class FF : IGate
    {
        private FF() { }

        private static FF fF = new FF();

        public bool Execute(bool[] inputs)
        {
            throw new NotImplementedException();
        }

        public CircuitType GetCircuitType()
        {
            return CircuitType.FF;
        }

        public static IGate GetInstance()
        {
            return fF;
        }
    }
}
