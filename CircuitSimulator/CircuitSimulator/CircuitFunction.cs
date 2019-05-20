using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator
{
    public class CircuitFunction
    {
        public static bool AND(int[] inputs)
        {
            foreach(var i in inputs)
            {
                if(i == 0) { return false; }
            }
            return true;
        }

        public static bool NAND(int[] inputs)
        {
            return !AND(inputs);
        }

        public static bool OR(int[] inputs)
        {
            foreach(var i in inputs)
            {
                if(i == 1) { return true; }
            }
            return false;
        }

        public static bool NOR(int[] inputs)
        {
            return !OR(inputs);
        }

        public static int EXOR()
        {
            return -1;
        }

        public static int FF()
        {
            return -1;
        }

        public static int NOT(int input)
        {
            if (input == 1) { return 0; }
            else { return 1; }
        }
    }
}
