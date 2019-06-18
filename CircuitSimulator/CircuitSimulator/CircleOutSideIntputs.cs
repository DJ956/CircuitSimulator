using System;
using System.Collections.Generic;

namespace CircuitSimulator
{
    public class CircleOutSideInputs
    {        
        public List<int> OutSideInputs { get; private set; }

        public CircleOutSideInputs(List<int> outsideInputs) { OutSideInputs = outsideInputs; }

        public override string ToString()
        {
            var result = "";
            foreach(var i in OutSideInputs)
            {
                result += i + "\n";
            }
            return result;
        }
    }
}
