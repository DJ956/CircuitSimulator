using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator
{
    public class CirclesRawData
    {
        public List<List<int>> CircleRawlist { get; private set; }
        public CircleInputs CircleInputs { get; private set; }
        public CircleOutSizeInputs CircleOutSideInputs { get; private set; }
        public CircleOutsideOutputs CirclesOutSideOutputs { get; private set; }

        public CirclesRawData(List<List<int>> circleRawlist, List<int> inputs, List<int> outsideInputs, List<int> outsideOutputs)
        {
            CircleRawlist = circleRawlist;
            CircleInputs = new CircleInputs(inputs);
            CircleOutSideInputs = new CircleOutSizeInputs(outsideInputs);
            CirclesOutSideOutputs = new CircleOutsideOutputs(outsideOutputs);
        }

    }
}
