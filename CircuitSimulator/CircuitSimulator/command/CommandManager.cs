using System;
using System.Collections.Generic;
using System.IO;

namespace CircuitSimulator.command
{
    public class CommandManager
    {
        private List<ICommand> commands;

        public CommandManager()
        {
            commands = new List<ICommand>();
            commands.Add(new StandAloneCommand());
            commands.Add(new ServerCommand());
            commands.Add(new ClientCommand());
            commands.Add(new ExitCommand());
        }

        public void Execute()
        {
            Console.WriteLine("サーバーとして実行:(s) クライアントとして実行:(c) 自機のみで実行:(sa) 終了:(ex)");
            var key = Console.ReadLine();

            foreach (var command in commands)
            {
                if (command.GetCommandType() == key) { command.Execute(); }
            }
        }

        public static void SaveResult(string tableName, List<CircleData> circles, List<List<bool>> answers, int faultCount, int detectCount)
        {
            Console.WriteLine($"故障数:{faultCount}");            
            Console.WriteLine($"故障検出数:{detectCount}");
            Console.WriteLine($"故障検出率:{(double)detectCount / faultCount * 100}");

            var outputFileName = tableName.Replace(".tbl", "_result.txt");
            DataIO.SaveResultAsync(answers, outputFileName).Wait();
            Console.WriteLine($"シミュレーション結果を{Path.Combine(DataIO.ROOT, outputFileName)}に保存しました。");

            outputFileName = tableName.Replace(".tbl", ".txt");
            DataIO.SaveCircleDataAsync(circles, outputFileName).Wait();
            Console.WriteLine($"シミュレーション済み回路データを{Path.Combine(DataIO.ROOT, outputFileName)}に保存しました。");

            //outputFileName = tableName.Replace(".tbl", "fault_result.txt");
            //DataIO.SaveFaultResultsAsync(outputFileName, faultResults).Wait();
            //Console.WriteLine($"故障シミュレーション結果を{Path.Combine(DataIO.ROOT, outputFileName)}に保存しました");
        }

        public static void Initialize(string tableName, string faultName, string patternName,
            out List<CircleData> circles, out CirclePatternes patternes, out List<List<bool>> answers, out List<CircleFault> faults,
            out CircuitPathFinder pathFinder, out CircleDataBuilder builder)
        {
            builder = new CircleDataBuilder();

            var circleRawData = DataIO.LoadTableAsync(tableName).Result;
            var circleInputs = circleRawData.CircleInputs;
            circles = builder.BuildCircles(circleRawData.CircleRawlist, circleInputs);
            faults = DataIO.LoadCircleFaultsFromTxtAsync(faultName).Result;

            pathFinder = new CircuitPathFinder(circles);

            //シミュレーション実行&結果出力            
            patternes = DataIO.LoadCirclePatternesFromTxtAsync(patternName, circleRawData.CircleOutSideInputs).Result;
            answers = new List<List<bool>>(patternes.Patternes.Count);

            foreach (var p in patternes.Patternes)
            {
                var result = pathFinder.Simulation(p, true);
                var tmp = new List<bool>(result);
                answers.Add(tmp);
            }
        }

    }
}
