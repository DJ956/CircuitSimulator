using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator.gate
{
    public class NAND : IGate
    {        
        private NAND() { }
        private static NAND nAND = new NAND();

        public static IGate GetInstance() { return nAND; }

        public bool Execute(bool[] inputs)
        {
            return !AND.GetInsctance().Execute(inputs);
        }

        public CircuitType GetCircuitType() { return CircuitType.NAND; }
    }
}
