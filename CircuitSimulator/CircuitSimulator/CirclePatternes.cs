using System;
using System.Collections.Generic;

namespace CircuitSimulator
{
    [Serializable]
    public class CirclePatternes
    {        
        public CirclePatternes() { }

        public List<List<int>> Patternes { get; set; }

        public CirclePatternes(List<List<int>> patternes) { Patternes = patternes; }
    }
}
