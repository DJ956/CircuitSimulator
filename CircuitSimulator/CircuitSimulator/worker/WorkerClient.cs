using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.Serialization;

namespace CircuitSimulator.worker
{
    public class WorkerClient
    {
        private static readonly int ANSWER_SPAN = 0;
        private static readonly int CIRCLE_SPAN = 1;
        private static readonly int PATTERN_SPAN = 2;
        private static readonly int FAULT_SPAN = 3;

        private string address;
        private int port;

        private TcpClient client;

        private CircuitPathFinder pathFinder;

        /// <summary>
        /// サーバーからの仕事を行う
        /// </summary>
        /// <param name="address">サーバーアドレス</param>
        /// <param name="port">サーバーポート</param>
        public WorkerClient(string address, int port)
        {
            this.address = address;
            this.port = port;
        }

        /// <summary>
        /// サーバーと接続したら作業に必要なデータを受信して故障シミュレーションを実行する
        /// </summary>
        /// <returns></returns>
        public async Task ConnectAndStartJob()
        {
            client = new TcpClient(address, port);
            Console.WriteLine($"サーバーと接続");
            using (var stream = client.GetStream())
            {
                var span = Encoding.UTF8.GetString(await ReadDataSpanAsync(stream, WorkerManager.SPAN_SIZE));
                Console.WriteLine("データスパン:" + span);
                var spans = span.Split(",");

                int dataSize = int.Parse(spans[ANSWER_SPAN]) + int.Parse(spans[CIRCLE_SPAN]) +
                    int.Parse(spans[PATTERN_SPAN]) + int.Parse(spans[FAULT_SPAN]);

                var src = await ReadDataSpanAsync(stream, dataSize);
                int seek = 0;

                var answersData = new byte[int.Parse(spans[ANSWER_SPAN])];
                Buffer.BlockCopy(src, seek, answersData, 0, answersData.Length);
                seek += int.Parse(spans[ANSWER_SPAN]);
                var circlesData = new byte[int.Parse(spans[CIRCLE_SPAN])];
                Buffer.BlockCopy(src, seek, circlesData, 0, circlesData.Length);
                seek += int.Parse(spans[CIRCLE_SPAN]);
                var patternData = new byte[int.Parse(spans[PATTERN_SPAN])];
                Buffer.BlockCopy(src, seek, patternData, 0, patternData.Length);
                seek += int.Parse(spans[PATTERN_SPAN]);
                var faultsData = new byte[int.Parse(spans[FAULT_SPAN])];
                Buffer.BlockCopy(src, seek, faultsData, 0, faultsData.Length);

                Console.WriteLine("答えデータ:" + answersData.Length);
                Console.WriteLine("回路データ:" + circlesData.Length);
                Console.WriteLine("パターンデータ:" + patternData.Length);
                Console.WriteLine("故障データ:" + faultsData.Length);

                Console.WriteLine("必要データ受信完了");
                Console.WriteLine("----------------------------------------");

                try
                {
                    var answers = DataIO.Deserialize(answersData) as List<List<bool>>;
                    var circles = DataIO.Deserialize(circlesData) as List<CircleData>;
                    var patterns = DataIO.Deserialize(patternData) as CirclePatternes;
                    var faults = DataIO.Deserialize(faultsData) as List<CircleFault>;

                    pathFinder = new CircuitPathFinder(circles);
                    var result = pathFinder.FaultSimulatorAsync(patterns, answers, faults);
                    Console.WriteLine($"ジョブ終了 結果:{result.Count(r => r == true)}");

                    await SendResultAsync(stream, result);
                    Console.WriteLine("データ送信完了");
                }
                catch (SerializationException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    Environment.Exit(-1);
                }
            }
        }

        /// <summary>
        /// データスパン受信
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private async Task<byte[]> ReadDataSpanAsync(NetworkStream stream, int size)
        {
            using(var nenory = new MemoryStream())
            {
                var buffer = new byte[size];
                await stream.ReadAsync(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        /// <summary>
        /// 答えデータ、回路データ、パターンデータ、故障データなどをひとまとまりで受信
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private async Task<byte[]> ReadDataAsync(NetworkStream stream)
        {
            using (var memory = new MemoryStream())
            {
                var buffer = new byte[4096];
                int resSize = 0;
                do
                {
                    resSize = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (resSize == 0) { break; }

                    await memory.WriteAsync(buffer, 0, resSize);
                } while (stream.DataAvailable);

                await memory.FlushAsync();
                return memory.ToArray();
            }
        }

        /// <summary>
        /// 故障シミュレーション結果を送信する
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task SendResultAsync(NetworkStream stream, List<bool> result)
        {
            var msg = $"{result.Count(r => r == true)}\n";
            var msgData = Encoding.UTF8.GetBytes(msg);
            await stream.WriteAsync(msgData, 0, msgData.Length);
            await stream.FlushAsync();
        }
    }
}
