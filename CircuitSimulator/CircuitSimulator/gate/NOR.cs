using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator.gate
{
    public class NOR : IGate
    {
        private NOR() { }
        private static NOR nOR = new NOR();

        public static IGate GetInstance() { return nOR; }

        public bool Execute(bool[] inputs)
        {
            return !OR.GetInstance().Execute(inputs);
        }

        public CircuitType GetCircuitType() { return CircuitType.NOR; }
    }
}
