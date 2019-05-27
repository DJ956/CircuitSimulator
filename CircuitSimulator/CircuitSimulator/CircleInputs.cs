﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CircuitSimulator
{
    public class CircleInputs
    {
        public static List<int> Inputs { get; private set; }

        private static readonly string FILENAME = "inputs_list.txt";

        static CircleInputs()
        {
            Inputs = new List<int>();
            Inputs = DataIO.LoadCircleInputFromTxtAsync(FILENAME).Result;
        }
    }
}
