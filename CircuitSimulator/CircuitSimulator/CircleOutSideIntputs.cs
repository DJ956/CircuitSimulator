using System;
using System.Collections.Generic;

namespace CircuitSimulator
{
    [Serializable]
    public class CircleOutSideInputs
    {
        public List<int> OutSideInputs { get; set; }

        public CircleOutSideInputs() { }

        public CircleOutSideInputs(List<int> outsideInputs) { OutSideInputs = outsideInputs; }
    }
}
