using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

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

            //ファイルパス入力
            Console.WriteLine("テーブルファイル名を入力してください(*.tbl)...");
            var tableFileName = Console.ReadLine();           
            Console.WriteLine("パターンファイル名を入力してください(*.pat)...");
            var patternFileName = Console.ReadLine();            
            Console.WriteLine("故障リストファイル名を入力してください(*.rep)...");
            var faultFileName = Console.ReadLine();

            Console.WriteLine("");
            Execute(tableFileName, patternFileName, faultFileName);
        }

        private static void Execute(string tableFileName, string patternFileName, string faultFileName)
        {            
            var start = DateTime.Now;

            //データ入力
            var builder = new CircleDataBuilder();            

            var circleRawData = DataIO.LoadTableAsync(tableFileName).Result;
            var circleInputs = circleRawData.CircleInputs;
            var circles = builder.BuildCircles(circleRawData.CircleRawlist, circleInputs);
            var faults = DataIO.LoadCircleFaultsFromTxtAsync(faultFileName).Result;

            var pathFinder = new CircuitPathFinder(circles);

            //シミュレーション実行&結果出力
            var outputFileName = tableFileName.Replace(".tbl", "_result.txt");
            var circlePatternes = DataIO.LoadCirclePatternesFromTxtAsync(patternFileName).Result;
            var answers = new List<List<bool>>(circlePatternes.Patternes.Count);

            foreach (var p in circlePatternes.Patternes)
            {
                var result = pathFinder.Simulation(p, true);
                var tmp = new List<bool>(result);                
                answers.Add(tmp);
            }
            Console.WriteLine($"論理シミュレーション時間:{(DateTime.Now - start).TotalSeconds}/s");

            //故障シミュレーション実行            
            var faultResults = pathFinder.FaultSimulatorAsync(circlePatternes, answers, faults);

            var end = DateTime.Now;
            var time = end - start;

            Console.WriteLine($"故障数:{faults.Count}");
            var detectCount = faultResults.AsParallel().Count(f => f == true);
            Console.WriteLine($"故障検出数:{detectCount}");            
            Console.WriteLine($"故障検出率:{(double)detectCount / faults.Count * 100}");

            DataIO.SaveResultAsync(answers, outputFileName).Wait();
            Console.WriteLine($"シミュレーション結果を{Path.Combine(DataIO.ROOT, outputFileName)}に保存しました。");

            outputFileName = tableFileName.Replace(".tbl", ".txt");
            DataIO.SaveCircleDataAsync(circles, outputFileName).Wait();
            Console.WriteLine($"シミュレーション済み回路データを{Path.Combine(DataIO.ROOT, outputFileName)}に保存しました。");

            outputFileName = tableFileName.Replace(".tbl", "fault_result.txt");
            DataIO.SaveFaultResultsAsync(outputFileName, faultResults).Wait();
            Console.WriteLine($"故障シミュレーション結果を{Path.Combine(DataIO.ROOT, outputFileName)}に保存しました");

            Console.WriteLine(time.TotalSeconds + "/s");
        }
    }
}
