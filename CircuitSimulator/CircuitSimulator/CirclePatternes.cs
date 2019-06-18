using System;
using System.Collections.Generic;

namespace CircuitSimulator
{
    [Serializable]
    public class CirclePatternes
    {        
        public CirclePatternes() { }

        /// <summary>
        /// Key=回路Index Value=回路の値
        /// </summary>
        public List<Dictionary<int, int>> Patternes { get; set; }

        public CirclePatternes(List<Dictionary<int, int>> keyValues)
        {
            Patternes = keyValues;
        }
    }
}
