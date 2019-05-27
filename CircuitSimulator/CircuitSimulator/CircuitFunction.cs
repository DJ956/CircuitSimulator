using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator
{
    public class CircuitFunction
    {
        public static bool AND(bool[] inputs)
        {
            foreach(var i in inputs)
            {
                if(i == false) { return false; }
            }
            return true;
        }

        public static bool NAND(bool[] inputs)
        {
            return !AND(inputs);
        }

        public static bool OR(bool[] inputs)
        {
            foreach(var i in inputs)
            {
                if(i == true) { return true; }
            }
            return false;
        }

        public static bool NOR(bool[] inputs)
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

        public static bool NOT(bool input)
        {
            return !input;
        }
    }
}
