using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
        public async static Task<List<List<int>>> LoadCircleFromTxtAsync(string fileName)
        {
            var path = Path.Combine(ROOT, fileName);

            List<List<int>> result = null;

            using (var reader = new StreamReader(path, false))
            {
                var count = int.Parse(await reader.ReadLineAsync());
                result = new List<List<int>>(count);

                var line = "";
                while ((line = await reader.ReadLineAsync()) != null)
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
        public async static Task<List<int>> LoadCircleInputFromTxtAsync(string fileName)
        {
            List<int> result = null;
            var path = Path.Combine(ROOT, fileName);
            using (var reader = new StreamReader(path, false))
            {
                var line = "";
                var count = int.Parse(await reader.ReadLineAsync());
                result = new List<int>(count);
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var data = int.Parse(line);
                    result.Add(data);
                }
            }
            return result;
        }

        /// <summary>
        /// txtから外部入力線を読み込む
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async static Task<List<int>> LoadCircleOutSideInputsFromTxtAsync(string fileName)
        {
            List<int> result = null;
            var path = Path.Combine(ROOT, fileName);
            using (var reader = new StreamReader(path, false))
            {
                var line = "";
                var count = int.Parse(await reader.ReadLineAsync());
                result = new List<int>(count);
                while ((line = await reader.ReadLineAsync()) != null)
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
        public async static Task<List<int>> LoadCircleOutsideOutputsFromTxtAsync(string fileName)
        {
            return await LoadCircleOutSideInputsFromTxtAsync(fileName);
        }

        /// <summary>
        /// txtから入力パターン読み込む。処理内容はリスト1の回路データを読み込むやつと同じなので使いまわす。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async static Task<List<List<int>>> LoadCirclePatternesFromTxtAsync(string fileName)
        {
            return await LoadCircleFromTxtAsync(fileName);
        }

        /// <summary>
        /// 論理シミュレータの出力結果を保存する
        /// </summary>
        /// <param name="results"></param>
        /// <param name="fileName"></param>
        public async static Task SaveResultAsync(List<CircleData> result, string fileName)
        {
            var path = Path.Combine(ROOT, fileName);
            using (var writer = new StreamWriter(path, true))
            {
                foreach (var c in result)
                {
                    var v = c.Value ? 1 : 0;
                    await writer.WriteAsync(v.ToString());
                    await writer.WriteAsync(" ");
                }
                await writer.WriteLineAsync("");
                await writer.FlushAsync();
            }
        }
    }
}
