using System;
using CircuitSimulator.gate;

namespace CircuitSimulator
{
    public struct CircleData
    {
        public int Index { get; private set; }
        public CircuitType CircuitType { get; private set; }
        public bool Already { get; private set; }
        public bool Value { get; set; }
        public int[] Inputs { get; private set; }
        public int[] Outs { get; private set; }

        private int priority;
        public int Priority
        {
            get { return priority; }
            set
            {
                priority = value;
                Already = true;
            }
        }

        public CircleData(int index, CircuitType type, int[] inputs, int[] outs)
        {
            Index = index;
            CircuitType = type;
            Inputs = inputs;
            Outs = outs;

            Already = false;
            Value = false;
            priority = -1;
        }

        public CircleData(int index, CircuitType type, int[] inputs, int[] outs, int priotiry)
        {
            Index = index;
            CircuitType = type;
            Inputs = inputs;
            Outs = outs;

            Already = false;
            Value = false;
            this.priority = -1;

            Priority = priotiry;
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
            return $"INDEX:{Index} Priority:{Priority} TYPE:{CircuitType} INPUTS:({inputstr}) OUTS:({outputstr.ToString()}) Already:{a} Value:{v}";
        }
    }
}
