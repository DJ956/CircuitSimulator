using CircuitSimulator.gate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace CircuitSimulator
{
    public class CircuitPathFinder
    {
        /// <summary>
        /// 答えソートリストを作成する
        /// </summary>
        /// <param name="circles"></param>
        public CircuitPathFinder(CircleData[] circles)
        {
            Circles = circles;
            indexDict = new Dictionary<int, int>(circles.Length);
            sortedIndex = new SortedDictionary<int, int>();
            for (int i = 0; i < circles.Length; i++)
            {
                var c = Circles[i];
                indexDict.Add(c.Index, i);
                if (c.CircuitType == CircuitType.PO)
                {
                    sortedIndex.Add(c.Inputs[0], i);
                }
            }
        }

        public CircleData[] Circles { get; private set; }

        /// <summary>
        /// POのソートされた配列Indexリスト
        /// </summary>
        private SortedDictionary<int, int> sortedIndex;

        /// <summary>
        /// Key = 回路Index ,Value = 配列Index
        /// </summary>
        private Dictionary<int, int> indexDict;

        /// <summary>
        /// 回路データの値のキャッシュ
        /// </summary>
        private List<List<bool>> circleValuesCash;

        /// <summary>
        /// 入力パターン選択,CircleDataのValueの初期化
        /// </summary>
        /// <param name="pattern">入力パターン</param>
        private void Initialize(Dictionary<int, int> pattern)
        {
            if (circleValuesCash == null) { circleValuesCash = new List<List<bool>>(); }
            //回路データの値の初期化
            Parallel.ForEach(Circles, c => c.Value = false);

            //外部入力値設定            
            //外部入力のリストを見て行わないと狂う。
            foreach (KeyValuePair<int, int> p in pattern)
            {
                var index = indexDict[p.Key];
                Circles[index].Value = p.Value == 1 ? true : false;
            }
        }

        /// <summary>
        /// CircleDataの入力Indexが格納されている配列からそれらの値を摘出します。
        /// </summary>        
        /// <param name="i">処理中の配列インデックス</param>
        /// <returns></returns>
        private bool[] SelectInputs(int i)
        {
            var inputsIndexes = Circles[i].Inputs;//入力のindexたち
            var inputsValues = new bool[inputsIndexes.Length]; //入力のindexたちが保有するvalue値
            for (int j = 0; j < inputsValues.Length; j++)
            {
                var index = inputsIndexes[j];
                var target = Circles[indexDict[index]];
                inputsValues[j] = target.Value;
            }

            return inputsValues;
        }

        /// <summary>
        /// 回路データを元に故障キャッシュから入力値を取得する(スレッドセーフ)
        /// </summary>        
        /// <param name="cash">故障キャッシュ</param>
        /// <param name="i">処理中の配列インデックス</param>
        /// <returns></returns>
        private bool[] GetInputsSafe(List<bool> cash, int i)
        {
            var inputsIndexes = Circles[i].Inputs;
            var result = new bool[inputsIndexes.Length];
            for (int j = 0; j < inputsIndexes.Length; j++)
            {
                var index = inputsIndexes[j];
                var targetIndex = indexDict[index];
                result[j] = cash[targetIndex];
            }

            return result;
        }

        /// <summary>
        /// CircleDataの値を決定する スレッドセーフではない
        /// </summary>
        /// <param name="circles">回路データ</param>
        /// <param name="i">処理中の配列インデックス</param>
        private void FindPath(int i)
        {
            var type = Circles[i].CircuitType;
            var inputs = SelectInputs(i);
            Circles[i].Value = GateFunctions.Execute(type, inputs);
        }

        /// <summary>
        /// ゲート演算結果を故障キャッシュに設定する(スレッドセーフ)
        /// </summary>        
        /// <param name="faultCash">故障キャッシュ</param>
        /// <param name="i">処理中の配列インデックス</param>
        /// <param name="fault">故障個所</param>
        private void DetectValueSafe(List<bool> faultCash, int i, CircleFault fault)
        {
            var c = Circles[i];
            var type = c.CircuitType;            
            var inputs = GetInputsSafe(faultCash, i);
            faultCash[i] = GateFunctions.Execute(type, inputs, c, fault);
        }

        /// <summary>
        /// 論理シミュレータを実行して、入力パターンから出力を得る。
        /// </summary>
        /// <param name="pattern">入力パターン</param>
        /// <param name="saveCash">キャッシュデータを保存するか否か</param>
        /// <returns>論理回路の出力結果</returns>
        public List<bool> Simulation(Dictionary<int, int> pattern, bool saveCash)
        {
            Initialize(pattern);
            List<bool> cash = null;

            if (saveCash) { cash = new List<bool>(Circles.Length); }
            for (int i = 0; i < Circles.Length; i++)
            {
                if (Circles[i].CircuitType != CircuitType.PI)
                {
                    FindPath(i);
                }
                if (saveCash) { cash.Add(Circles[i].Value); }
            }

            //出力のみを取得とキャッシュデータ保存
            var result = new List<bool>(sortedIndex.Count);
            foreach (KeyValuePair<int, int> item in sortedIndex)
            {
                result.Add(Circles[item.Value].Value);
            }
            //キャッシュのリストの追加
            if (saveCash) { circleValuesCash.Add(cash); }

            return result;
        }

        /// <summary>
        /// 途中で伝搬しなくなれば故障シミュレーションを止める。
        /// </summary>
        /// <param name="fault">故障する場所</param>
        /// <param name="cash">正常値のキャッシュ</param>
        /// <returns></returns>
        private bool SimulationRouteSafe(CircleFault fault, List<bool> cash)
        {
            var faultCash = new List<bool>(cash);
            var arrayIndex = indexDict[fault.FaultIndex];
            faultCash[arrayIndex] = fault.FaultValue;

            var path = new Tuple<int, int>(Circles[arrayIndex].Priority, arrayIndex);

            //Tuple<int, int> Item1 = Priority Item2 = Index
            var routes = new PriorityQueue<Tuple<int, int>>((x, y) => x.Item1 - y.Item1);
            routes.Enqueue(path);

            var detect = false;
            do
            {
                path = routes.Dequeue();
                //先頭の伝搬が終わるまでやる
                var c = Circles[path.Item2];
                if (c.CircuitType != CircuitType.PI)
                {
                    DetectValueSafe(faultCash, path.Item2, fault);
                }
                //ヘッドがPOかつ伝搬すれば検出可能
                if (c.CircuitType == CircuitType.PO && faultCash[path.Item2] != cash[path.Item2])
                {
                    detect = true;
                    break;
                }
                //伝搬しなければ次のヘッドを取りに行く
                if (faultCash[path.Item2] == cash[path.Item2]) { continue; }

                //伝搬すれば
                var outs = c.Outs;
                foreach (var o in outs)
                {
                    arrayIndex = indexDict[o];
                    var item = new Tuple<int, int>(c.Priority, arrayIndex);
                    routes.Enqueue(item);
                }

            } while (routes.Any());

            return detect;
        }

        /// <summary>
        /// 非同期処理で故障シミュレーションを行う
        /// </summary>                
        /// <param name="answers">正常時の答えリスト</param>
        /// <param name="faults">故障リスト</param>
        /// <returns></returns>
        public IEnumerable<bool> FaultSimulatorAsync(List<List<bool>> answers, CircleFault[] faults, int threadCount)
        {
            var result = new BlockingCollection<bool>();
            var options = new ParallelOptions() { MaxDegreeOfParallelism = threadCount };
            //すべての故障個所が発見できたか走査する。
            Parallel.ForEach(faults, options, f =>
            {
                var isDetect = false;
                //全入力パターンを回して検出できるか試す。検出できたならそこで終了
                for (int i = 0; i < circleValuesCash.Count; i++)
                {
                    //正常値
                    var answer = answers[i];
                    var cash = circleValuesCash[i];

                    //最初に発生した故障自体が元と同じなら終了する
                    var index = indexDict[f.FaultIndex];
                    if (cash[index] == f.FaultValue) { continue; }

                    //故障設定した状態でシミュレーション実行                
                    isDetect = SimulationRouteSafe(f, cash);

                    //検知できていれば入力パターン走査を終了する
                    if (isDetect) { break; }
                }
                //検出可不可を保存
                result.TryAdd(isDetect);
            });

            return result;
        }

        /// <summary>
        /// 並列化せずに故障シミュレーション実行
        /// </summary>
        /// <param name="answers"></param>
        /// <param name="faults"></param>
        /// <returns></returns>
        public IEnumerable<bool> FaultSimulator(List<List<bool>> answers, CircleFault[] faults)
        {
            var result = new List<bool>(faults.Length);
            //すべての故障個所が発見できたか走査する。            
            foreach (var f in faults)
            {
                var isDetect = false;
                //全入力パターンを回して検出できるか試す。検出できたならそこで終了
                for (int i = 0; i < circleValuesCash.Count; i++)
                {
                    //正常値
                    var answer = answers[i];
                    var cash = circleValuesCash[i];

                    //最初に発生した故障自体が元と同じなら終了する
                    var index = indexDict[f.FaultIndex];
                    if (cash[index] == f.FaultValue) { continue; }

                    //故障設定した状態でシミュレーション実行                                        
                    isDetect = SimulationRouteSafe(f, cash);

                    //検知できていれば入力パターン走査を終了する
                    if (isDetect) { break; }
                }
                //検出可不可を保存
                result.Add(isDetect);
            }
            return result;
        }
    }
}
