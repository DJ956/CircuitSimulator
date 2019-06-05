using CircuitSimulator.gate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            if(circleValuesCash == null) { circleValuesCash = new List<List<bool>>(); }

            //回路データの値の初期化
            Parallel.ForEach(circles, c => c.Value = false);
            //foreach (var c in circles) { c.Value = false; }

            //外部入力値設定
            var i = 0;
            foreach (var p in pattern)
            {
                var value = p == 1 ? true : false;
                circles[i].Value = value;
                i++;
            }
        }

        /// <summary>
        /// 入力パターン選択、CircleDataのValueの初期化 故障の設定(GateFunctionsに細工をする)
        /// </summary>
        /// <param name="circles">回路データ</param>
        /// <param name="circlePatternes">入力パターンリスト</param>
        /// <param name="selectPatternIndex">使用する入力パターン</param>
        /// <param name="fault">故障位置</param>
        private void Initialize(List<CircleData> circles, List<int> pattern, CircleFault fault)
        {
            Initialize(circles, pattern);
            GateFunctions.CircleFault = fault;
        }

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


        /// <summary>
        /// CircleDataの入力Indexが格納されている配列からそれらの値を摘出します。
        /// </summary>
        /// <param name="circles"></param>
        /// <param name="i"></param>
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
        /// CircleDataの値を決定する
        /// </summary>
        /// <param name="circles"></param>
        /// <param name="i"></param>
        private void FindPath(List<CircleData> circles, int i)
        {
            var type = circles[i].CircuitType;
            var inputs = SelectInputs(circles, i);
            circles[i].Value = GateFunctions.Execute(type, inputs, circles[i]);
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
        /// 
        /// </summary>
        /// <param name="circles"></param>
        /// <param name="pattern"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public List<CircleData> Simulation(List<CircleData> circles, CircleFault fault)
        {
            GateFunctions.CircleFault = fault;
            var startIndex = circles.FindIndex(c => c.Index == fault.FaultIndex);
            for(int i = startIndex; i < circles.Count; i++)
            {
                FindPath(circles, i);
            }

            var result = new List<CircleData>();
            foreach(var c in circles)
            {
                if(c.CircuitType == CircuitType.PO) { result.Add(c); }
            }
            result.Sort((a, b) => a.Inputs[0] - b.Inputs[0]);
            return result;
        }

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

        public List<bool> FaultSimulatorOld(List<CircleData> circles, CirclePatternes circlePatternes, List<List<bool>> answers,
            List<CircleFault> faults)
        {
            var result = new List<bool>();

            //すべての故障個所が発見できたか走査する。
            //Parallel.ForEach(faults, f =>           
            foreach (var f in faults)
            {
                var isDetect = false;
                //全入力パターンを回して検出できるか試す。検出できたならそこで終了

                for (int i = 0; i < circlePatternes.Patternes.Count; i++)
                {
                    //正常値
                    var answer = answers[i];
                    var p = circlePatternes.Patternes[i];
                    var cash = circleValuesCash[i];

                    //故障個所設定 
                    //Initialize(circles, p, f);
                    GateFunctions.CircleFault = f;

                    for (int j = 0; j < circles.Count; j++) { circles[j].Value = cash[j]; }

                    //故障設定した状態でシミュレーション実行
                    var simResult = Simulation(circles, f);

                    //答え合わせ
                    for (int j = 0; j < answer.Count; j++)
                    {
                        if (simResult[j].Value != answer[j])
                        {
                            isDetect = true;
                            break;
                        }
                    }

                    //検知できていれば入力パターン走査を終了する
                    if (isDetect) { break; }
                }
                //検出可不可を保存
                result.Add(isDetect);
            }

            //この後の処理でSimulatorを実行しても故障が誤発生しないようにする。
            GateFunctions.CircleFault = null;

            return result;
        }

    }
}
