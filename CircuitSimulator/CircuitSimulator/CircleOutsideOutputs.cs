using System;
using System.Collections.Generic;

namespace CircuitSimulator
{
    public class CircleOutsideOutputs
    {        
        public int[] OutsideOutputs { get; private set; }

        public CircleOutsideOutputs(int[] outsideOutputs) { OutsideOutputs = outsideOutputs; }
    }
}
