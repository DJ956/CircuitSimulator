using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator
{
    public class CircleTypeDict
    {
        public static Dictionary<int, CircuitType> TypeMap { get; private set; }

        static CircleTypeDict()
        {
            TypeMap = new Dictionary<int, CircuitType>();
            TypeMap.Add(0, CircuitType.PI);
            TypeMap.Add(1, CircuitType.OR);
            TypeMap.Add(2, CircuitType.AND);
            TypeMap.Add(3, CircuitType.branch);
            TypeMap.Add(4, CircuitType.PO);
            TypeMap.Add(5, CircuitType.EXOR);
            TypeMap.Add(6, CircuitType.FF);
            TypeMap.Add(-1, CircuitType.NAND);
            TypeMap.Add(-2, CircuitType.NOR);
            TypeMap.Add(-3, CircuitType.NOT);
            TypeMap.Add(9, CircuitType.unuse);
        }

        public static CircuitType GetType(int code)
        {
            return TypeMap[code];
        }

    }
}
