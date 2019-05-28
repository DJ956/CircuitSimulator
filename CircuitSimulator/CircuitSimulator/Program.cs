using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;

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

            //以前に作成した結果ファイルを削除
            var oldFile = Path.Combine(DataIO.ROOT, tableFileName.Replace(".tbl", "_result.txt"));
            if(File.Exists(oldFile)) { File.Delete(oldFile); }

            var start = DateTime.Now;

            //データ入力
            var builder = new CircleDataBuilder();
            var pathFinder = new CircuitPathFinder();

            var circleRawData = DataIO.LoadTableAsync(tableFileName).Result;
            var circleInputs = circleRawData.CircleInputs;
            var circles = builder.BuildCircles(circleRawData.CircleRawlist, circleInputs);            
            
            //シミュレーション実行&結果出力
            var outputFileName = tableFileName.Replace(".tbl", "_result.txt");            
            var circlePatternes = DataIO.LoadCirclePatternesFromTxtAsync(patternFileName).Result;
            
            for (int i = 0; i < circlePatternes.Patternes.Count; i++)
            {
                var result = pathFinder.Simulation(circles, circlePatternes, i);
                DataIO.SaveResultAsync(result, outputFileName).Wait();                
            }
            var end = DateTime.Now;
            var time = end - start;

            Console.WriteLine($"シミュレーション結果を{Path.Combine(DataIO.ROOT, outputFileName)}に保存しました。");

            outputFileName = tableFileName.Replace(".tbl", ".txt");
            DataIO.SaveCircleDataAsync(circles, outputFileName).Wait();
            Console.WriteLine($"シミュレーション済み回路データを{Path.Combine(DataIO.ROOT, outputFileName)}に保存しました。");                                    
           
            Console.WriteLine(time.TotalSeconds + "/s");
        }
    }
}
