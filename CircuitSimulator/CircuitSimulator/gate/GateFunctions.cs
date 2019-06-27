using System;
using System.Collections.Generic;

namespace CircuitSimulator.gate
{
    public class GateFunctions
    {
        //private static List<IGate> gates;
        private static Dictionary<CircuitType, IGate> gates;

        static GateFunctions()
        {
            /*
            gates = new List<IGate>(9);
            gates.Add(AND.GetInsctance());
            gates.Add(Branch.GetInstance());
            gates.Add(EXOR.GetInstance());            
            gates.Add(NAND.GetInstance());
            gates.Add(NOR.GetInstance());
            gates.Add(NOT.GetInstance());
            gates.Add(OR.GetInstance());
            gates.Add(PI.GetInstance());
            gates.Add(PO.GetInstance());           
            */
            gates = new Dictionary<CircuitType, IGate>(8);
            gates.Add(CircuitType.PI, PI.GetInstance());
            gates.Add(CircuitType.PO, PO.GetInstance());
            gates.Add(CircuitType.AND, AND.GetInsctance());
            gates.Add(CircuitType.branch, Branch.GetInstance());
            gates.Add(CircuitType.EXOR, EXOR.GetInstance());
            gates.Add(CircuitType.NAND, NAND.GetInstance());
            gates.Add(CircuitType.NOR, NOR.GetInstance());
            gates.Add(CircuitType.NOT, NOT.GetInstance());
            gates.Add(CircuitType.OR, OR.GetInstance());            
        }

        /// <summary>
        /// ゲート演算を行う。故障のインデックスを指定すれば、その故障値を返す。
        /// </summary>
        /// <param name="circuitType">ゲートの種類</param>
        /// <param name="inputs">入力値</param>
        /// <param name="index">故障が発生するインデックス</param>
        /// <returns></returns>
        public static bool Execute(CircuitType circuitType, bool[] inputs, CircleData circle, CircleFault fault)
        {
            //故障個所のインデックスになれば故障値を返す。
            if(fault.FaultIndex == circle.Index) { return fault.FaultValue; }

            //return gates.Find(g => g.GetCircuitType() == circuitType).Execute(inputs);                        
            return gates[circuitType].Execute(inputs);
        }

        /// <summary>
        /// ゲート演算を行う
        /// </summary>
        /// <param name="circuitType">ゲートの種類</param>
        /// <param name="inputs">ゲートへの入力値</param>
        /// <returns></returns>
        public static bool Execute(CircuitType circuitType, bool[] inputs)
        {
            //return gates.Find(g => g.GetCircuitType() == circuitType).Execute(inputs);
            return gates[circuitType].Execute(inputs);            
        }
    }
}
