using CircuitSimulator.gate;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public CircuitPathFinder(List<CircleData> circles)
        {            
            Circles = circles;
            sortedIndex = new SortedDictionary<int, int>();
            for (int i = 0; i < circles.Count; i++)
            {
                var c = Circles[i];
                if (c.CircuitType == CircuitType.PO)
                {
                    sortedIndex.Add(c.Inputs[0], i);
                }
            }
        }

        public List<CircleData> Circles { get; private set; }        

        private SortedDictionary<int, int> sortedIndex;

        /// <summary>
        /// 回路データの値のキャッシュ
        /// </summary>
        private List<List<bool>> circleValuesCash;

        /// <summary>
        /// 入力パターン選択,CircleDataのValueの初期化
        /// </summary>
        /// <param name="circles"></param>
        /// <param name="patternes"></param>
        /// <param name="selectPatternIndex"></param>
        private void Initialize(Dictionary<int, int> pattern)
        {
            if (circleValuesCash == null) { circleValuesCash = new List<List<bool>>(); }
            //回路データの値の初期化
            //Parallel.ForEach(Circles, c => c.Value = false);
            foreach(var c in Circles) { c.Value = false; }
            
            //外部入力値設定            
            //外部入力のリストを見て行わないと狂う。
            foreach (KeyValuePair<int, int> p in pattern)
            {                                               
                var index = Circles.FindIndex(c => c.Index == p.Key);
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
                var target = Circles.Find(c => c.Index == index);
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
        private bool[] GetInputsSafe(bool[] cash, int i)
        {
            var inputsIndexes = Circles[i].Inputs;
            var result = new bool[inputsIndexes.Length];
            for (int j = 0; j < inputsIndexes.Length; j++)
            {
                var index = inputsIndexes[j];
                var targetIndex = Circles.FindIndex(c => c.Index == index);
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
        private void DetectValueSafe(bool[] faultCash, int i, CircleFault fault)
        {
            var type = Circles[i].CircuitType;
            var inputs = GetInputsSafe(faultCash, i);
            faultCash[i] = GateFunctions.Execute(type, inputs, Circles[i], fault);
        }

        /// <summary>
        /// 論理シミュレータを実行して、入力パターンから出力を得る。
        /// </summary>
        /// <param name="circles">回路データリスト</param>
        /// <param name="pattern">入力パターン</param>
        /// <param name="saveCash">キャッシュデータを保存するか否か</param>
        /// <returns>論理回路の出力結果</returns>
        public List<bool> Simulation(Dictionary<int, int> pattern, bool saveCash)
        {
            Initialize(pattern);
            List<bool> cash = null;

            if (saveCash) {cash = new List<bool>(Circles.Count); }
            for (int i = 0; i < Circles.Count; i++)
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
        /// スレッドセーフな故障シミュレーション
        /// </summary>        
        /// <param name="pattern"></param>
        /// <param name="fault"></param>
        /// <returns></returns>
        private List<bool> SimulationSafe(Dictionary<int, int> pattern, CircleFault fault)
        {                        
            var faultCash = new bool[Circles.Count];
            //入力パターン設定
            foreach (KeyValuePair<int, int> p in pattern)
            {
                var index = Circles.FindIndex(c => c.Index == p.Key);
                faultCash[index] = p.Value == 1 ? true : false;
            }
            
            for (int i = 0; i < Circles.Count; i++)
            {
                DetectValueSafe(faultCash, i, fault);
            }
                        
            var result = new List<bool>(sortedIndex.Count);
            foreach (KeyValuePair<int, int> item in sortedIndex)
            {
                result.Add(faultCash[item.Value]);
            }

            return result;
        }


        private bool SimulationSafe(CircleFault fault, List<int> faultPath, List<bool> cash)
        {
            var faultCash = new bool[Circles.Count];
            //値設定
            for (int i = 0; i < faultCash.Length; i++)
            {
                faultCash[i] = cash[i];
            }

            faultPath.Sort((a, b) =>
            {
                var ai = Circles.FindIndex(c => c.Index == a);
                var bi = Circles.FindIndex(c => c.Index == b);
                return Circles[ai].Priority - Circles[bi].Priority;
            });

            var allClear = new List<bool>(faultPath.Count);
            foreach (var fp in faultPath)
            {
                var i = Circles.FindIndex(c => c.Index == fp);
                DetectValueSafe(faultCash, i, fault);

                if (cash[i] == faultCash[i])
                {
                    allClear.Add(false);
                }
                else { allClear.Add(true); }
            }

            var count = allClear.Count(a => a == false);
            
            if (faultPath.Count == count)
            {
                return false;
            }
            else { return true; }
        }

        /// <summary>
        /// 非同期処理で故障シミュレーションを行う
        /// </summary>        
        /// <param name="circlePatternes">入力パターン</param>
        /// <param name="answers">正常時の答えリスト</param>
        /// <param name="faults">故障リスト</param>
        /// <returns></returns>
        public List<bool> FaultSimulatorAsync(CirclePatternes circlePatternes, List<List<bool>> answers,
            List<CircleFault> faults)
        {                        
            var result = new BlockingCollection<bool>();
            //すべての故障個所が発見できたか走査する。
            Parallel.ForEach(faults, f =>            
            {
                var isDetect = false;
                //全入力パターンを回して検出できるか試す。検出できたならそこで終了
                for (int i = 0; i < circlePatternes.Patternes.Count; i++)
                {
                    //正常値
                    var answer = answers[i];
                    var p = circlePatternes.Patternes[i];
                    var cash = circleValuesCash[i];

                    //最初に発生した故障自体が元と同じなら終了する
                    var index = Circles.FindIndex(c => c.Index == f.FaultIndex);
                    if(cash[index] == f.FaultValue) { continue; }

                    //故障設定した状態でシミュレーション実行
                    var simResult = SimulationSafe(p, f);

                    //答え合わせ
                    for (int j = 0; j < answer.Count; j++)
                    {
                        if (simResult[j] != answer[j])
                        {
                            isDetect = true;
                            break;
                        }
                    }

                    //検知できていれば入力パターン走査を終了する
                    if (isDetect) { break; }
                }
                //検出可不可を保存
                result.TryAdd(isDetect);
            });

            return result.ToList();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faultPaths">Key=故障回路Index Valueが回路Indexの経路</param>
        /// <param name="answers"></param>
        /// <param name="circlePatternes"></param>
        /// <param name="faults"></param>
        /// <returns></returns>
        public List<bool> FaultSimulator(Dictionary<CircleFault, List<int>> faultPaths, CirclePatternes circlePatternes,
            List<CircleFault> faults)
        {
            var result = new BlockingCollection<bool>();
            //Parallel.ForEach(faults, f =>
            foreach (var f in faults)
            {
                var isDetect = false;

                for (int i = 0; i < circlePatternes.Patternes.Count; i++)
                {
                    //var p = circlePatternes.Patternes[i];
                    var faultPath = faultPaths[f];
                    var cash = circleValuesCash[i];

                    //最初に発生した故障自体が元と同じなら終了する
                    var index = Circles.FindIndex(c => c.Index == f.FaultIndex);
                    if (cash[index] == f.FaultValue) { continue; }

                    isDetect = SimulationSafe(f, faultPath, cash);
                    if (isDetect)
                    {
                        break;
                    }
                }
                result.TryAdd(isDetect);
            }//);

            return result.ToList();
        }
    }
}
