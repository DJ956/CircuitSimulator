using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CircuitSimulator
{
    public class CircleOutsideOutputs
    {
        public static List<int> OutsideOutputs { get; private set; }

        private static readonly string FILENAME = "outside_outputs.txt";

        static CircleOutsideOutputs()
        {
            OutsideOutputs = new List<int>();
            OutsideOutputs = DataIO.LoadCircleOutsideOutputsFromTxt(FILENAME);
        }
    }
}
