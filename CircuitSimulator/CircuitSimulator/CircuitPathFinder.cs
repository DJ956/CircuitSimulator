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
        public CircuitPathFinder() { }

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
        private void Initialize(List<CircleData> circles, List<int> pattern)
        {
            if (circleValuesCash == null) { circleValuesCash = new List<List<bool>>(); }

            //回路データの値の初期化
            Parallel.ForEach(circles, c => c.Value = false);
            //foreach (var c in circles) { c.Value = false; }

            //外部入力値設定            
            for (int i = 0; i < pattern.Count; i++)
            {
                var p = pattern[i];                
                circles[i].Value = p == 1 ? true : false;                
            }
        }

        /*
        /// <summary>
        /// 回路データと故障個所、正常値を見て等価故障かどうかを調べる
        /// すべての故障個所からの出力が、等価故障だった場合、最後まで見る必要はない
        /// </summary>
        /// <returns>故障が検知可能()ならTrue,不可能(全て等価故障)ならFalse</returns>        
        private bool EquivalentFailure(List<CircleData> circles, int patternIndex, CircleFault fault)
        {
            var equivalent = true;

            //キャッシュデータの復元
            var cash = circleValuesCash[patternIndex];
            //故障キャッシュ
            var faultCash = new List<bool>(cash.Count);
            for (int i = 0; i < circles.Count; i++)
            {
                circles[i].Value = cash[i];
                faultCash[i] = cash[i];
            }

            var faultCircle = circles.Find(c => c.Index == fault.FaultIndex);
            var outputs = faultCircle.Outs;
            var tasks = new Queue<int>(outputs);

            do
            {
                var faultIndex = tasks.Dequeue();
                faultCircle = circles.Find(c => c.Index == faultIndex);
                //正常値の値をキャッシュから復元
                for (int i = 0; i < cash.Count; i++) { circles[i].Value = cash[i]; }
                var normalInputs = SelectInputs(circles, faultIndex);

                //故障発生を起こす
                for (int i = 0; i < faultCash.Count; i++) { circles[i].Value = faultCash[i]; }//Index違い
                circles.Find(c => c.Index == faultIndex).Value = fault.FaultValue;
                var faultInputs = SelectInputs(circles, faultIndex);
                //故障キャッシュにデータ記録する
                faultCash[circles.FindIndex(c => c.Index == faultIndex)] = GateFunctions.Execute(faultCircle.CircuitType, faultInputs);

                //等価故障ではなかった場合                
                if (!GateFunctions.EquivalentFailure(faultCircle.CircuitType, normalInputs, faultInputs))
                {
                    equivalent = false;
                    //見ているところが、最終外部出力でなければ故障出力追加                    
                    if (faultCircle.CircuitType == CircuitType.PO) { continue; }
                    foreach (var fo in faultCircle.Outs) { tasks.Enqueue(fo); }
                }
                else { equivalent = true; }

            } while (tasks.Any());
            //故障した場所からの出力を見る

            //回路データに入力されたキャッシュデータもとに戻しておく
            foreach (var c in circles) { c.Value = false; }

            return equivalent;
        }
        */

        /// <summary>
        /// CircleDataの入力Indexが格納されている配列からそれらの値を摘出します。
        /// </summary>
        /// <param name="circles">回路データ</param>
        /// <param name="i">処理中の配列インデックス</param>
        /// <returns></returns>
        private bool[] SelectInputs(List<CircleData> circles, int i)
        {
            var inputsIndexes = circles[i].Inputs;//入力のindexたち
            var inputsValues = new bool[inputsIndexes.Length]; //入力のindexたちが保有するvalue値
            for (int j = 0; j < inputsValues.Length; j++)
            {
                var index = inputsIndexes[j];
                var target = circles.Find(c => c.Index == index);
                inputsValues[j] = target.Value;
            }

            return inputsValues;
        }

        /// <summary>
        /// 回路データを元に故障キャッシュから入力値を取得する(スレッドセーフ)
        /// </summary>
        /// <param name="circles">回路データ　読み取り専用</param>
        /// <param name="cash">故障キャッシュ</param>
        /// <param name="i">処理中の配列インデックス</param>
        /// <returns></returns>
        private bool[] GetInputsSafe(List<CircleData> circles, bool[] cash, int i)
        {
            var inputsIndexes = circles[i].Inputs;
            var result = new bool[inputsIndexes.Length];
            for (int j = 0; j < inputsIndexes.Length; j++)
            {
                var index = inputsIndexes[j];
                var targetIndex = circles.FindIndex(c => c.Index == index);
                result[j] = cash[targetIndex];
            }

            return result;
        }

        /// <summary>
        /// CircleDataの値を決定する スレッドセーフではない
        /// </summary>
        /// <param name="circles">回路データ</param>
        /// <param name="i">処理中の配列インデックス</param>
        private void FindPath(List<CircleData> circles, int i)
        {
            var type = circles[i].CircuitType;
            var inputs = SelectInputs(circles, i);
            circles[i].Value = GateFunctions.Execute(type, inputs);
        }

        /// <summary>
        /// ゲート演算結果を故障キャッシュに設定する(スレッドセーフ)
        /// </summary>
        /// <param name="circles">回路データ　読み込み処理のみ</param>
        /// <param name="faultCash">故障キャッシュ</param>
        /// <param name="i">処理中の配列インデックス</param>
        /// <param name="fault">故障個所</param>
        private void DetectValueSafe(List<CircleData> circles, bool[] faultCash, int i, CircleFault fault)
        {
            var type = circles[i].CircuitType;
            var inputs = GetInputsSafe(circles, faultCash, i);
            faultCash[i] = GateFunctions.Execute(type, inputs, circles[i], fault);
        }

        /// <summary>
        /// 論理シミュレータを実行して、入力パターンから出力を得る。
        /// </summary>
        /// <param name="circles">回路データリスト</param>
        /// <param name="pattern">入力パターン</param>
        /// <param name="saveCash">キャッシュデータを保存するか否か</param>
        /// <returns>論理回路の出力結果</returns>
        public List<CircleData> Simulation(List<CircleData> circles, List<int> pattern, bool saveCash)
        {
            Initialize(circles, pattern);

            for (int i = 0; i < circles.Count; i++)
            {
                FindPath(circles, i);
            }

            var cash = new List<bool>(circles.Count);
            //出力のみを取得とキャッシュデータ保存
            var result = new List<CircleData>();
            foreach (var c in circles)
            {
                if (c.CircuitType == CircuitType.PO) { result.Add(c); }
                cash.Add(c.Value);
            }
            //キャッシュのリストの追加
            if (saveCash) { circleValuesCash.Add(cash); }

            result.Sort((a, b) => a.Inputs[0] - b.Inputs[0]);
            return result;
        }

        /// <summary>
        /// スレッドセーフな故障シミュレーション
        /// </summary>
        /// <param name="circles"></param>
        /// <param name="pattern"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private List<bool> SimulationSafe(List<CircleData> circles, List<int> pattern, CircleFault fault)
        {
            var faultCash = new bool[circles.Count];
            //入力パターン設定
            for (int i = 0; i < pattern.Count; i++)
            {
                faultCash[i] = pattern[i] == 1 ? true : false;
            }

            for (int i = 0; i < circles.Count; i++)
            {
                DetectValueSafe(circles, faultCash, i, fault);
            }

            //key => InputIndex Value=>配列のインデックス
            var dic = new SortedDictionary<int, int>();

            for (int i = 0; i < circles.Count; i++)
            {
                var c = circles[i];
                if (c.CircuitType == CircuitType.PO)
                {
                    dic.Add(c.Inputs[0], i);
                }
            }

            var result = new List<bool>(dic.Count);
            foreach (KeyValuePair<int, int> item in dic)
            {
                result.Add(faultCash[item.Value]);
            }

            return result;
        }
        /*
        /// <summary>
        /// 故障シミュレーションを実行する
        /// </summary>
        /// <param name="circles">回路データ</param>
        /// <param name="circlePatternes">入力パターン</param>        
        /// <param name="answers">正常な状態での全入力パターンに対する値</param>
        /// <param name="faults">故障個所リスト</param>
        /// <returns></returns>
        public List<bool> FaultSimulator(List<CircleData> circles, CirclePatternes circlePatternes, List<List<bool>> answers,
            List<CircleFault> faults)
        {
            var result = new List<bool>(faults.Count);

            //すべての故障個所が発見できたか走査する。

            foreach (var f in faults)
            {
                var isDetect = false;
                //全入力パターンを回して検出できるか試す。検出できたならそこで終了                
                for (int i = 0; i < circlePatternes.Patternes.Count; i++)
                {
                    //正常値
                    var answer = answers[i];
                    var p = circlePatternes.Patternes[i];

                    //故障した個所からのすべての出力が等価故障だったので検知不可能
                    isDetect = EquivalentFailure(circles, i, f);
                }
                //検出可不可を保存
                result.Add(isDetect);
            }

            //この後の処理でSimulatorを実行しても故障が誤発生しないようにする。
            GateFunctions.CircleFault = null;

            return result;
        }
        */

        /// <summary>
        /// 非同期処理で故障シミュレーションを行う
        /// </summary>
        /// <param name="circles">回路データ　読み取り専用</param>
        /// <param name="circlePatternes">入力パターン</param>
        /// <param name="answers">正常時の答えリスト</param>
        /// <param name="faults">故障リスト</param>
        /// <returns></returns>
        public List<bool> FaultSimulatorAsync(List<CircleData> circles, CirclePatternes circlePatternes, List<List<bool>> answers,
            List<CircleFault> faults)
        {
            var result = new BlockingCollection<bool>();
            //すべての故障個所が発見できたか走査する。
            Parallel.ForEach(faults, f =>
            //foreach (var f in faults)
            {
                var isDetect = false;
                //全入力パターンを回して検出できるか試す。検出できたならそこで終了
                for (int i = 0; i < circlePatternes.Patternes.Count; i++)
                {
                    //正常値
                    var answer = answers[i];
                    var p = circlePatternes.Patternes[i];                    

                    //故障設定した状態でシミュレーション実行
                    var simResult = SimulationSafe(circles, p, f);

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
    }
}
