using System;
using System.Collections.Generic;

namespace CircuitSimulator
{
    public class CirclePatternes
    {        
        /// <summary>
        /// Key=回路Index Value=回路の値
        /// </summary>
        public List<Dictionary<int, int>> Patternes { get; private set; }

        public CirclePatternes(List<Dictionary<int, int>> keyValues)
        {
            Patternes = keyValues;
        }
    }
}
