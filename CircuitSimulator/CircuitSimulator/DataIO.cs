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

            using(var reader = new StreamReader(path, false))
            {
                var count = int.Parse(reader.ReadLine());
                result = new List<List<int>>(count);

                var line = "";
                while ((line = reader.ReadLine()) != null)
                {
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
            var result = new List<int>();
            var path = Path.Combine(ROOT, fileName);
            using(var reader = new StreamReader(path, false))
            {
                var line = "";
                while((line = reader.ReadLine()) != null)
                {
                    var data = int.Parse(line);
                    result.Add(data);
                }
            }
            return result;
        }
    }
}
