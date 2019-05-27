using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator.gate
{
    public class AND : IGate
    {
        private AND() { }
        private static AND aND = new AND(); 

        public CircuitType GetCircuitType() { return CircuitType.AND; }

        public static IGate GetInsctance()
        {
            return aND;
        }

        public bool Execute(bool[] inputs)
        {
            foreach (var i in inputs)
            {
                if (i == false) { return false; }
            }
            return true;
        }
    }
}
