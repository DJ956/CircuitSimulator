using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator.gate
{
    public class PI : IGate
    {
        private PI() { }
        private static PI pI = new PI();

        public static IGate GetInstance() { return pI; }

        public bool Execute(bool[] inputs)
        {
            if(inputs.Length > 1) { throw new ArgumentException($"PIに対する入力の数が不正です。入力数:{ inputs.Length }"); }
            return inputs[0];
        }

        public CircuitType GetCircuitType() { return CircuitType.PI; }

        public bool EquivalentFailure(bool[] normalInputs, bool[] faultInputs)
        {
            return Execute(normalInputs) == Execute(faultInputs);
        }
    }
}
