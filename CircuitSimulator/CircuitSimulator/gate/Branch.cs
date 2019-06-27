using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator.gate
{
    public class Branch : IGate
    {
        private Branch() { }

        private static Branch branch = new Branch();

        public static IGate GetInstance() { return branch; }

        public bool Execute(bool[] inputs)
        {            
            if (inputs.Length > 1) { throw new ArgumentException($"Branchに対する入力の数が不正です。入力数:{inputs.Length}"); }
            return inputs[0];            
        }

        public CircuitType GetCircuitType() { return CircuitType.branch; }
    }
}
