using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator.gate
{
    public class NOT : IGate
    {
        private NOT() { }
        private static NOT nOT = new NOT();

        public static IGate GetInstance() { return nOT; }

        public bool Execute(bool[] inputs)
        {
            if(inputs.Length > 1) { throw new ArgumentException($"NOTに対する入力の数が不正です。入力数:{inputs.Length}"); }
            return !inputs[0];
        }

        public CircuitType GetCircuitType() { return CircuitType.NOT; }

        public bool EquivalentFailure(bool[] normalInputs, bool[] faultInputs)
        {
            return Execute(normalInputs) == Execute(faultInputs);
        }
    }
}
