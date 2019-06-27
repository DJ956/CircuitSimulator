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
    }
}
