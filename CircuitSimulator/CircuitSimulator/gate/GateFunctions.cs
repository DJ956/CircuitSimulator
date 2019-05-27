using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator.gate
{
    public class GateFunctions
    {
        private static List<IGate> gates;

        static GateFunctions()
        {
            gates = new List<IGate>();

            gates.Add(AND.GetInsctance());
            gates.Add(Branch.GetInstance());
            gates.Add(EXOR.GetInstance());
            gates.Add(FF.GetInstance());
            gates.Add(NAND.GetInstance());
            gates.Add(NOR.GetInstance());
            gates.Add(NOT.GetInstance());
            gates.Add(OR.GetInstance());
            gates.Add(PI.GetInstance());
            gates.Add(PO.GetInstance());
        }

        public static bool Execute(CircuitType circuitType, bool[] inputs)
        {
            foreach(var g in gates)
            {
                if(g.GetCircuitType() == circuitType)
                {
                    return g.Execute(inputs);
                }
            }

            throw new Exception($"ゲート:{circuitType}に対する処理が実装されていません");
        }
    }
}
