using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CircuitSimulator
{
    public class CirclePatternes
    {
        public static List<List<int>> Patternes { get; private set; }

        private static readonly string FILENAME = "ex5.pat";

        static CirclePatternes()
        {
            Patternes = new List<List<int>>();
            var path = Path.Combine(DataIO.ROOT, FILENAME);
            Patternes = DataIO.LoadCirclePatternesFromTxtAsync(path).Result;
        }
    }
}
