using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator
{
    public class CircleData
    {
        public int Index { get; private set; }
        public CircuitType CircuitType { get; private set; }
        public bool Already { get; set; } = false;
        public bool Value { get; set; } = false;
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
            var inputstr = "";
            foreach (var input in Inputs) { inputstr += input + " "; }
            inputstr = inputstr.TrimEnd();
            var outputstr = "";
            foreach (var output in Outs) { outputstr += output + " "; }
            outputstr = outputstr.TrimEnd();
            var a = Already ? 1 : 0;
            var v = Value ? 1 : 0;
            return $"INDEX:{Index} TYPE:{CircuitType} INPUTS:({inputstr}) OUTS:({outputstr.ToString()}) Already:{a} Value:{v}";
        }
    }
}
