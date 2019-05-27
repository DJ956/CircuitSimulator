using System;
using System.Collections.Generic;

namespace CircuitSimulator
{
    public class CircleOutSizeInputs
    {
        public List<int> OutSideInputs { get; private set; }

        public CircleOutSizeInputs(List<int> outsideInputs)
        {
            OutSideInputs = outsideInputs;
        }
    }
}
