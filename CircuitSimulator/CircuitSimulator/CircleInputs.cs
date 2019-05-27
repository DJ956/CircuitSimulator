using System;
using System.Collections.Generic;

namespace CircuitSimulator
{
    public class CircleInputs
    {        
        public List<int> Inputs { get; private set; }

        public CircleInputs(List<int> inputs) { Inputs = inputs; }
    }
}
