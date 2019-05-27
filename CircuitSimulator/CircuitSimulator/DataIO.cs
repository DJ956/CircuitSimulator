using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CircuitSimulator
{
    public class DataIO
    {
        public static readonly string ROOT = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Circle");
        private static readonly string SPLIT = " ";

        static DataIO()
        {
            if (!Directory.Exists(ROOT)) { Directory.CreateDirectory(ROOT); }
        }

        /// <summary>
        /// リスト1の回路データを読み込む
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<List<int>> LoadCircleFromTxt(string fileName)
        {
            var path = Path.Combine(ROOT, fileName);

            List<List<int>> result = null;

            using (var reader = new StreamReader(path, false))
            {
                var count = int.Parse(reader.ReadLine());
                result = new List<List<int>>(count);

                var line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.TrimStart();
                    var lines = line.Split(SPLIT);
                    var row = new List<int>(lines.Length);
                    foreach (var str in lines)
                    {
                        row.Add(int.Parse(str));
                    }
                    result.Add(row);
                }
            }
            return result;
        }

        /// <summary>
        /// リスト2の入力線を読み込む
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="inputs"></param>
        public static List<int> LoadCircleInputFromTxt(string fileName)
        {
            List<int> result = null;
            var path = Path.Combine(ROOT, fileName);
            using (var reader = new StreamReader(path, false))
            {
                var line = "";
                var count = int.Parse(reader.ReadLine());
                result = new List<int>(count);
                while ((line = reader.ReadLine()) != null)
                {
                    var data = int.Parse(line);
                    result.Add(data);
                }
            }
            return result;
        }

        /// <summary>
        /// txtから外部出力線を読み込む
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<int> LoadCircleOutSideInputsFromTxt(string fileName)
        {
            List<int> result = null;
            var path = Path.Combine(ROOT, fileName);
            using (var reader = new StreamReader(path, false))
            {
                var line = "";
                var count = int.Parse(reader.ReadLine());
                result = new List<int>(count);
                while ((line = reader.ReadLine()) != null)
                {
                    var data = int.Parse(line);
                    result.Add(data);
                }
            }
            return result;
        }

        public static List<int> LoadCircleOutsideOutputsFromTxt(string fileName)
        {
            return LoadCircleOutSideInputsFromTxt(fileName);
        }

        /// <summary>
        /// 入力パターン読み込む
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<List<int>> LoadCirclePatternesFromTxt(string fileName)
        {
            return LoadCircleFromTxt(fileName);
        }

        /// <summary>
        /// 論理シミュレータの出力結果を保存する
        /// </summary>
        /// <param name="results"></param>
        /// <param name="fileName"></param>
        public async static void SaveResultAsync(List<CircleData> results, string fileName)
        {
            var path = Path.Combine(ROOT, fileName);
            using(var writer = new StreamWriter(path, false))
            {
                foreach(var c in results)
                {
                    var v = c.Value ? 1 : 0;
                    await writer.WriteAsync(v.ToString());
                    await writer.WriteAsync(" ");
                }
                await writer.FlushAsync();
            }
        }
    }
}
