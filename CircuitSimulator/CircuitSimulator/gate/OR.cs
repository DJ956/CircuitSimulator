using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator.gate
{
    public class OR : IGate
    {
        private OR() { }
        private static OR oR = new OR();

        public static IGate GetInstance() { return oR; }

        public CircuitType GetCircuitType() { return CircuitType.OR; }

        public bool Execute(bool[] inputs)
        {
            foreach (var i in inputs)
            {
                if (i == true) { return true; }
            }
            return false;
        }

        public bool EquivalentFailure(bool[] normalInputs, bool[] faultInputs)
        {
            return Execute(normalInputs) == Execute(faultInputs);
        }
    }
}
