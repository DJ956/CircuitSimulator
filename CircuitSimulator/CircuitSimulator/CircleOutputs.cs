using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CircuitSimulator
{
    public class CircleOutputs
    {
        public static List<int> Outputs { get; private set; }

        private static readonly string FILENAME = "outputs_list.txt";

        static CircleOutputs()
        {
            Outputs = new List<int>();
            var path = Path.Combine(DataIO.ROOT, FILENAME);
            Outputs = DataIO.LoadCircleOutputFromTxt(path);
        }
    }
}
