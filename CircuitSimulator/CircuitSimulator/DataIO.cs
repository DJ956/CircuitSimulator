using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
            CirclesRawData result = null;
            try
            {
                using (var reader = new StreamReader(path, false))
                {
                    var circleRaw = await LoadCircleFromTxtAsync(reader);
                    var inputs = await LoadCircleInputFromTxtAsync(reader);
                    var outsideInputs = await LoadCircleOutSideInputsFromTxtAsync(reader);
                    var outsideOutputs = await LoadCircleOutsideOutputsFromTxtAsync(reader);

                    result = new CirclesRawData(circleRaw, inputs, outsideInputs, outsideOutputs);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(-1);
            }
            return result;
        }

        /// <summary>
        /// 故障ファイルから故障リストを読み込む
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async static Task<List<CircleFault>> LoadCircleFaultsFromTxtAsync(string fileName)
        {
            List<CircleFault> result = null;
            var path = Path.Combine(ROOT, fileName);
            try
            {
                using (var reader = new StreamReader(path, false))
                {
                    var count = int.Parse(await reader.ReadLineAsync());
                    result = new List<CircleFault>(count);
                    for (int i = 0; i < count; i++)
                    {
                        var line = await reader.ReadLineAsync();
                        line = line.TrimStart();
                        var lines = line.Split(SPLIT);
                        if (lines.Length > 2) { throw new FormatException($"{fileName}のフォーマットが不正です。"); }

                        var faultIndex = int.Parse(lines[0]);
                        var faultValue = lines[1] == "1" ? true : false;
                        result.Add(new CircleFault(faultIndex, faultValue));
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(-1);
            }

            return result;
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
            for (int i = 0; i < count; i++)
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
            try
            {
                char split = ' ';
                var path = Path.Combine(ROOT, fileName);
                using (var reader = new StreamReader(path, false))
                {
                    var count = int.Parse(await reader.ReadLineAsync());
                    results = new List<List<int>>(count);

                    for (int i = 0; i < count; i++)
                    {
                        var line = await reader.ReadLineAsync();
                        line = line.TrimStart();
                        var lines = line.Split(split);
                        var row = new List<int>(lines.Length);
                        foreach (var str in lines)
                        {
                            row.Add(int.Parse(str));
                        }
                        results.Add(row);
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(-1);
            }
            return new CirclePatternes(results);
        }

        /// <summary>
        /// シミュレーション済みやロードした回路データを出力します。
        /// </summary>
        /// <param name="circles"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async static Task SaveCircleDataAsync(List<CircleData> circles, string fileName)
        {
            var path = Path.Combine(ROOT, fileName);
            try
            {
                using (var writer = new StreamWriter(path, false))
                {
                    foreach (var c in circles)
                    {
                        await writer.WriteLineAsync(c.ToString());
                    }
                    await writer.FlushAsync();
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("ファイルの書き込みに失敗しました\n" + ex.Message);
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// 論理シミュレータの出力結果を保存する
        /// </summary>
        /// <param name="answers"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async static Task SaveResultAsync(List<List<bool>> answers, string fileName)
        {
            var path = Path.Combine(ROOT, fileName);
            try
            {
                using (var writer = new StreamWriter(path, false))
                {
                    foreach (var answer in answers)
                    {
                        await writer.WriteAsync(" ");
                        foreach (var a in answer)
                        {
                            var v = a ? 1 : 0;
                            await writer.WriteAsync(v.ToString());
                            await writer.WriteAsync(" ");
                        }
                        await writer.WriteLineAsync("");
                    }
                    await writer.FlushAsync();
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("ファイルの書き込みに失敗しました\n" + ex.Message);
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// 故障診断結果をファイルに書き込む
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public async static Task SaveFaultResultsAsync(string fileName, List<bool> results)
        {
            var path = Path.Combine(ROOT, fileName);
            try
            {
                using (var writer = new StreamWriter(path, false))
                {
                    int index = 0;
                    foreach (var result in results)
                    {
                        await writer.WriteLineAsync($"{index}:{result}");
                        index++;
                    }
                    await writer.FlushAsync();
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("ファイルの書き込みに失敗しました\n" + ex.Message);
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Objectをバイナリーにシリアライズする
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Serialize(object obj)
        {
            var binary = new BinaryFormatter();
            using (var memory = new MemoryStream())
            {
                binary.Serialize(memory, obj);
                memory.Seek(0, SeekOrigin.Begin);
                return memory.ToArray();
            }
        }

        /// <summary>
        /// バイナリー化されたシリアライズデータをObjectに直す
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object Deserialize(byte[] data)
        {
            var binary = new BinaryFormatter();
            using (var memory = new MemoryStream(data))
            {
                return binary.Deserialize(memory);
            }
        }
    }
}
