using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator
{
    public interface IGate
    {        
        CircuitType GetCircuitType();        
        bool Execute(bool[] inputs);
    }
}
