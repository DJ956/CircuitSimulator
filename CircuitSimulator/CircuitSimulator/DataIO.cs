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
        private static readonly string SPLIT = "	";

        static DataIO()
        {
            if (!Directory.Exists(ROOT)) { Directory.CreateDirectory(ROOT); }
        }

        /// <summary>
        /// テーブルデータを読み込む(*.tbl)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async static Task<CirclesRawData> LoadTableAsync(string fileName)
        {
            var path = Path.Combine(ROOT, fileName);
            using(var reader = new StreamReader(path, false))
            {
                var circleRaw = await LoadCircleFromTxtAsync(reader);
                var inputs = await LoadCircleInputFromTxtAsync(reader);
                var outsideInputs = await LoadCircleOutSideInputsFromTxtAsync(reader);
                var outsideOutputs = await LoadCircleOutsideOutputsFromTxtAsync(reader);

                return new CirclesRawData(circleRaw, inputs, outsideInputs, outsideOutputs);
            }
        }


        /// <summary>
        /// リスト1の回路データを読み込む
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async static Task<List<List<int>>> LoadCircleFromTxtAsync(StreamReader reader)
        {
            List<List<int>> result = null;

            //最初の空行を読み飛ばす
            await reader.ReadLineAsync();
            //データ数読み取り
            var count = int.Parse(await reader.ReadLineAsync());
            result = new List<List<int>>(count);

            for (int i = 0; i < count; i++)
            {
                var line = await reader.ReadLineAsync();
                //データ加工
                line = line.TrimStart();
                var lines = line.Split(SPLIT);

                var row = new List<int>(lines.Length);
                foreach (var str in lines)
                {
                    row.Add(int.Parse(str));
                }
                result.Add(row);

            }
            return result;
        }

        /// <summary>
        /// リスト2の入力線を読み込む
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="inputs"></param>
        private async static Task<List<int>> LoadCircleInputFromTxtAsync(StreamReader reader)
        {
            List<int> result = null;
            //最初の空行を読み飛ばす
            await reader.ReadLineAsync();

            var count = int.Parse(await reader.ReadLineAsync());
            result = new List<int>(count);
            for(int i = 0; i < count; i++)
            {
                var line = await reader.ReadLineAsync();
                line = line.TrimStart();

                var data = int.Parse(line);
                result.Add(data);
            }

            return result;
        }

        /// <summary>
        /// txtから外部入力線を読み込む
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async static Task<List<int>> LoadCircleOutSideInputsFromTxtAsync(StreamReader reader)
        {
            return await LoadCircleInputFromTxtAsync(reader);
        }

        /// <summary>
        /// txtから外部出力線を読み込む
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async static Task<List<int>> LoadCircleOutsideOutputsFromTxtAsync(StreamReader reader)
        {
            return await LoadCircleOutSideInputsFromTxtAsync(reader);
        }

        /// <summary>
        /// txtから入力パターン読み込む。処理内容はリスト1の回路データを読み込むやつと同じなので使いまわす。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async static Task<CirclePatternes> LoadCirclePatternesFromTxtAsync(string fileName)
        {
            List<List<int>> results = null;

            char split = ' ';
            var path = Path.Combine(ROOT, fileName);
            using(var reader = new StreamReader(path, false))
            {
                var count = int.Parse(await reader.ReadLineAsync());
                results = new List<List<int>>(count);

                for(int i = 0; i < count; i++)
                {
                    var line = await reader.ReadLineAsync();
                    line = line.TrimStart();
                    var lines = line.Split(split);
                    var row = new List<int>(lines.Length);
                    foreach(var str in lines)
                    {
                        row.Add(int.Parse(str));
                    }
                    results.Add(row);
                }
            }
            return new CirclePatternes(results);
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
