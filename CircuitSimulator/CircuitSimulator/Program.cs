using System;
using System.Diagnostics;
using System.Reflection;
using CircuitSimulator.command;

namespace CircuitSimulator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            Console.WriteLine($"CircuitSimulator:{version}");
            Console.WriteLine($"作業ディレクトリ:{DataIO.ROOT}");
            Console.WriteLine("----------------------------------------");

            var manager = new CommandManager();
            manager.Execute();
        }
    }
}
