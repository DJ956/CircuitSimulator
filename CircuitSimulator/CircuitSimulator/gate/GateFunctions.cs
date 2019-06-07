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

            foreach(var g in gates)
            {
                if(g.GetCircuitType() == circuitType)
                {                                       
                    return g.Execute(inputs);
                }
            }

            throw new Exception($"ゲート:{circuitType}に対する処理が実装されていません");
        }

        /// <summary>
        /// ゲート演算を行う
        /// </summary>
        /// <param name="circuitType">ゲートの種類</param>
        /// <param name="inputs">ゲートへの入力値</param>
        /// <returns></returns>
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

        /// <summary>
        /// 等価故障が発生しているか判断する。
        /// </summary>
        /// <param name="circuitType">ゲートの種類</param>
        /// <param name="normalInputs">正常時での入力値</param>
        /// <param name="faultInputs">故障時での入力値</param>
        /// <returns></returns>
        public static bool EquivalentFailure(CircuitType circuitType, bool[] normalInputs, bool[] faultInputs)
        {
            foreach(var g in gates)
            {
                if(g .GetCircuitType() == circuitType)
                {
                    return g.EquivalentFailure(normalInputs, faultInputs);
                }
            }

            throw new Exception($"ゲート:{circuitType}に対する処理が実装されていません");
        }
    }
}
