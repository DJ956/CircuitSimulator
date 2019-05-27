using System;

namespace CircuitSimulator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            var builder = new CircleDataBuilder();
            var pathFinder = new CircuitPathFinder();

            var circleRawData = DataIO.LoadTableAsync("ex5.tbl").Result;

            var circleInputs = circleRawData.CircleInputs;
            var circles = builder.BuildCircles(circleRawData.CircleRawlist, circleInputs);

            foreach (var circle in circles)
            {
                Console.WriteLine(circle.ToString());
            }
            
            var circlePatternes = DataIO.LoadCirclePatternesFromTxtAsync("ex5.pat").Result;
            for (int i = 0; i < circlePatternes.Patternes.Count; i++)
            {
                var result = pathFinder.Simulation(circles, circlePatternes, i);
                DataIO.SaveResultAsync(result, "result.txt").Wait();
            }
            
        }
    }
}
