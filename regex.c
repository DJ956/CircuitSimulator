using System;
using System.IO;
using System.Text.RegularExpressions;

namespace RegexTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var name = Console.ReadLine();
            var root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Circle");
            var path = Path.Combine(root, name);

            using(var reader = new StreamReader(path, false))
            {
                var count = int.Parse(reader.ReadLine());
                Console.WriteLine(count);

                for(int i = 0; i < count; i++)
                {
                    var lines = reader.ReadLine();
                    var maches = Regex.Matches(lines, @"[\d]+");
                    Console.WriteLine($"Index:{maches[0]} Value:{maches[1]}");
                    
                    Console.WriteLine("////////////");
                }
            }
        }
    }
}
