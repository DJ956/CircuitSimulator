using System;
using System.Collections.Generic;

namespace CircuitSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var builder = new CircleDataBuilder();
            var circles = builder.BuildCircles("circles.txt");
            
            foreach(var circle in circles)
            {
                Console.WriteLine(circle.ToString());
            }

            var pathFinder = new CircuitPathFinder();
            pathFinder.FindPath(circles);
        }
    }
}
