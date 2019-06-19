using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace CircuitSimulator.worker
{
    public class WorkerManager
    {
        public static readonly int SPAN_SIZE = 50;

        private TcpListener listener;
        private int port;
        private int workerCount;

        private List<List<bool>> answers;
        private List<CircleFault> faults;
        private List<CircleData> circles;
        private CirclePatternes patternes;
        private CircleOutSideInputs outSideInputs;

        /// <summary>
        /// 故障シミュレーションを並列分散処理で行う
        /// </summary>
        /// <param name="port">サーバーのポート</param>
        /// <param name="workerCount">クライアントの数</param>
        /// <param name="answers">答えリスト</param>
        /// <param name="circles">回路データ</param>
        /// <param name="patternes">入力パターン</param>
        /// <param name="faults">故障リスト</param>
        public WorkerManager(int port, int workerCount, List<List<bool>> answers, List<CircleData> circles,
            CirclePatternes patternes, List<CircleFault> faults, CircleOutSideInputs outSideInputs)
        {
            this.workerCount = workerCount;
            this.answers = answers;            
            this.circles = circles;
            this.patternes = patternes;
            this.faults = faults;
            this.port = port;
            this.outSideInputs = outSideInputs;
            listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
        }

        /// <summary>
        /// 故障リストを分割する。
        /// </summary>
        /// <param name="count">ワーカーの個数(実際はこれに自機分の+1がなされる)</param>
        /// <returns></returns>
        private List<List<CircleFault>> SplitFaults(int count)
        {
            //自機も含めるため1増やす
            count = count + 1;

            var result = new List<List<CircleFault>>(count);
            var tasks = new Queue<CircleFault>(faults);

            var splitCount = faults.Count / count;

            for (int i = 0; i < count; i++)
            {
                var list = new List<CircleFault>();
                for (int j = 0; j < splitCount; j++)
                {
                    list.Add(tasks.Dequeue());                    
                }
                result.Add(list);
            }

            //余り物を加える
            while (tasks.Any())
            {
                result[0].Add(tasks.Dequeue());
            }

            return result;
        }

        /// <summary>
        /// サーバー起動
        /// </summary>
        /// <returns></returns>
        public async Task<int> StartAsync()
        {
            listener.Start();

            var workers = new List<TcpClient>();
            var ownAddress = "";
            foreach(var address in await Dns.GetHostAddressesAsync(Dns.GetHostName()))
            {
                if(address.AddressFamily == AddressFamily.InterNetwork) { ownAddress = address.ToString(); }
            }
            Console.WriteLine($"サーバースタート:{ownAddress}:{port}");

            //引数で受け取ったクライアント数の接続がくるまで繰り返す。            
            do
            {
                var client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                workers.Add(client);
                Console.WriteLine($"[{workers.Count}/{workerCount}] ワーカ追加:{client.Client.RemoteEndPoint}");
            } while (workers.Count != workerCount);

            Console.WriteLine("ワーク開始");
            var start = DateTime.Now;
            var result = ExecuteWork(workers);
            var end = DateTime.Now;

            Console.WriteLine($"処理時間:{(end - start).TotalSeconds} /s");
            return result;
        }

        /// <summary>
        /// 故障シミュレーションを並列分散処理で実行する。
        /// </summary>
        /// <param name="workers"></param>
        /// <returns></returns>
        private int ExecuteWork(List<TcpClient> workers)
        {
            var result = 0;
            var circleData = DataIO.Serialize(circles);
            var answersData = DataIO.Serialize(answers);
            var patternData = DataIO.Serialize(patternes);
            var splitFaults = SplitFaults(workers.Count);
            var outsideInputsData = DataIO.Serialize(outSideInputs);
            
            Parallel.Invoke(() =>
            {
                Parallel.For(0, workers.Count, i =>
                {
                    //ここからはクライアントが仕事をする。
                    var worker = workers[i];
                    var jobFault = splitFaults[i];

                    using (var stream = worker.GetStream())
                    {
                        //仕事内容送信
                        SendCircleData(stream, answersData, circleData, patternData, jobFault, outsideInputsData);
                        //仕事結果受信
                        var count = ReceiveResult(stream);
                        Interlocked.Add(ref result, count);
                        Console.WriteLine($"{worker.Client.RemoteEndPoint}のワーク完了");
                    }
                    worker.Close();
                });
            },
            //サーバーも故障シミュレーションを実行する
                () =>
                {
                    var job = splitFaults.Last();
                    var pathFinder = new CircuitPathFinder(circles, null);
                    var cnt = pathFinder.FaultSimulatorAsync(answers, job);
                    Interlocked.Add(ref result, cnt.Count(r => r == true));
                    Console.WriteLine($"自機のワーク完了");
                }
            );

            return result;
        }

        /// <summary>
        /// クライアントに仕事を投げる
        /// </summary>
        /// <param name="circles"></param>
        /// <param name="faults"></param>
        /// <returns></returns>
        private void SendCircleData(NetworkStream stream, byte[] answersData, byte[] circlesData, byte[] patternData,
            List<CircleFault> faults, byte[] outsideInputsData)
        {
            var faultData = DataIO.Serialize(faults);

            //データプロトコル
            var span = $"{answersData.Length},{circlesData.Length},{patternData.Length},{faultData.Length},{outsideInputsData.Length}";
            if (span.Length < SPAN_SIZE)
            {
                do
                {
                    span += ",";
                } while (span.Length != SPAN_SIZE);
            }
            var spanData = Encoding.UTF8.GetBytes(span);            
            stream.Write(spanData, 0, spanData.Length);
            Console.WriteLine("データスパン送信:" + span);

            stream.Write(answersData, 0, answersData.Length);            
            Console.WriteLine("答えデータ送信:" + answersData.Length);

            stream.Write(circlesData, 0, circlesData.Length);            
            Console.WriteLine("回路データ送信:" + circlesData.Length);

            stream.Write(patternData, 0, patternData.Length);            
            Console.WriteLine("パターンデータ送信:" + patternData.Length);

            stream.Write(faultData, 0, faultData.Length);            
            Console.WriteLine($"故障データ({faults.Count})送信:" + faultData.Length);

            stream.Write(outsideInputsData, 0, outsideInputsData.Length);
            Console.WriteLine("外部入力データ送信:" + outsideInputsData.Length);

            stream.Flush();
        }

        /// <summary>
        /// クライアントからの故障数を受信する
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private int ReceiveResult(NetworkStream stream)
        {
            var buffer = new byte[64];
            int resSize = 0;
            using(var memory = new MemoryStream())
            {
                do
                {
                    resSize = stream.Read(buffer, 0, buffer.Length);
                    if(resSize == 0) { break; }
                    memory.Write(buffer, 0, resSize);
                } while (stream.DataAvailable);

                memory.Flush();
                var msg = Encoding.UTF8.GetString(memory.ToArray(), 0, (int)memory.Length);
                return int.Parse(msg);
            }            
        }
    }
}
