﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CircuitSimulator.command
{
    public class AsyncCommand : ICommand
    {
        public AsyncCommand() { }

        public void Execute()
        {
            Console.WriteLine("ファイル名を入力してください...");
            var fileName = Console.ReadLine();
            var tableName = fileName + ".tbl";
            var patternName = fileName + ".pat";
            var faultName = fileName + "f.rep";

            //データ入力
            List<CircleData> circles;
            List<List<bool>> answers;
            List<CircleFault> faults;
            CircuitPathFinder pathFinder;

            var start = DateTime.Now;

            CommandManager.Initialize(tableName, faultName, patternName,
                out circles, out answers, out faults, out pathFinder);

            //故障シミュレーション実行            
            var faultResults = pathFinder.FaultSimulatorAsync(answers, faults);
            var end = DateTime.Now;
            Console.WriteLine($"処理時間:{(end - start).TotalSeconds}/s");

            var detectCount = faultResults.Count(f => f == true);

            CommandManager.SaveResult(tableName, circles, answers, faults.Count, detectCount);
        }

        public string GetCommandType()
        {
            return "as";
        }
    }
}
