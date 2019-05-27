using System;

namespace CircuitSimulator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var patternes = CirclePatternes.Patternes;
            var builder = new CircleDataBuilder();
            var pathFinder = new CircuitPathFinder();

            var circles = builder.BuildCirclesAsync("circles.txt").Result;

            foreach (var circle in circles)
            {
                Console.WriteLine(circle.ToString());
            }

            for (int i = 0; i < patternes.Count; i++)
            {
                var result = pathFinder.Simulation(circles, patternes, i);
                DataIO.SaveResultAsync(result, "result.txt").Wait();
            }
        }
    }
}
