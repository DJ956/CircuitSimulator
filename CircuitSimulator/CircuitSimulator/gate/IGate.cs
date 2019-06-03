using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator.gate
{
    public interface IGate
    {        
        CircuitType GetCircuitType();
        
        /// <summary>
        /// ゲート演算
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        bool Execute(bool[] inputs);

        /// <summary>
        /// 正常な入力値と、故障した場合での入力値による出力結果の違いを調べる。
        /// </summary>
        /// <param name="normalInputs">正常時での入力値</param>
        /// <param name="faultInputs">故障時での入力値</param>
        /// <returns></returns>
        bool EquivalentFailure(bool[] normalInputs, bool[] faultInputs);
    }
}
