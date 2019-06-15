using CircuitSimulator.worker;
using System;
using System.Collections.Generic;

namespace CircuitSimulator.command
{
    public class ServerCommand : ICommand
    {
        public ServerCommand() { }

        public void Execute()
        {
            Console.WriteLine("ファイル名を入力してください...");
            var fileName = Console.ReadLine();
            var tableName = fileName + ".tbl";
            var patternName = fileName + ".pat";
            var faultName = fileName + "f.rep";
            Console.WriteLine("----------------------------------------");

            Console.WriteLine("ポートを設定してください");
            var port = int.Parse(Console.ReadLine());
            Console.WriteLine("ワーカ数を入力してください");
            var workerCount = int.Parse(Console.ReadLine());

            List<CircleData> circles;
            CirclePatternes circlePatternes;
            List<List<bool>> answers;
            List<CircleFault> faults;
            CircuitPathFinder pathFinder;
            CircleDataBuilder builder;

            CommandManager.Initialize(tableName, faultName, patternName,
                out circles, out circlePatternes, out answers, out faults, out pathFinder, out builder);

            var detectCount = -1;
            try
            {
                var manager = new WorkerManager(port, workerCount, answers, circles, circlePatternes, faults);
                var start = DateTime.Now;
                detectCount = manager.StartAsync().Result;
                var end = DateTime.Now;
                Console.WriteLine($"処理時間:{(end - start).TotalSeconds}/s");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(-1);
            }

            CommandManager.SaveResult(tableName, circles, answers, faults.Count, detectCount);
        }

        public string GetCommandType()
        {
            return "s";
        }
    }
}
