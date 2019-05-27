using System;
using System.Collections.Generic;

namespace CircuitSimulator
{
    public class CirclePatternes
    {
        public List<List<int>> Patternes { get; private set; }

        public CirclePatternes(List<List<int>> patternes)
        {
            Patternes = patternes;
        }
    }
}
