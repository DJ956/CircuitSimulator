using System;
using System.Collections.Generic;

namespace CircuitSimulator
{
    public class CircleOutsideOutputs
    {
        public List<int> OutsideOutputs { get; private set; }

        public CircleOutsideOutputs(List<int> outsideOutputs)
        {
            OutsideOutputs = outsideOutputs;
        }
    }
}
