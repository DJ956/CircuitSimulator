using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator
{
    public class CircleData
    {
        public int Index { get; private set; }
        public CircuitType CircuitType { get; private set; }
        public int[] Inputs { get; private set; }
        public int[] Outs { get; private set; }

        public CircleData(int index, CircuitType type, int[] inputs, int[] outs)
        {
            Index = index;
            CircuitType = type;
            Inputs = inputs;
            Outs = outs;
        }

        public override string ToString()
        {
            return $"INDEX:{Index} TYPE:{CircuitType} INPUTS:({Inputs}) OUTS:({Outs})";
        }
    }
}
