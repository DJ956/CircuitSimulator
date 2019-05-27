using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CircuitSimulator
{
    public class CircleOutSizeInputs
    {
        public static List<int> OutSideInputs { get; private set; }

        private static readonly string FILENAME = "outside_inputs.txt";

        static CircleOutSizeInputs()
        {
            OutSideInputs = new List<int>();
            var path = Path.Combine(DataIO.ROOT, FILENAME);
            OutSideInputs = DataIO.LoadCircleOutSideInputsFromTxtAsync(path).Result;
        }
    }
}
