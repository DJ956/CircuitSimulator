using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator.gate
{
    public class PO : IGate
    {
        private PO() { }
        private static PO pO = new PO();

        public static IGate GetInstance() { return pO; }

        public bool Execute(bool[] inputs)
        {
            if (inputs.Length > 1) { throw new ArgumentException($"PIに対する入力の数が不正です。入力数:{ inputs.Length }"); }
            return inputs[0];
        }

        public CircuitType GetCircuitType() { return CircuitType.PO; }
    }
}
