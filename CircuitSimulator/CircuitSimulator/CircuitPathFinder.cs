using CircuitSimulator.gate;
using System;
using System.Collections.Generic;

namespace CircuitSimulator
{
    public class CircuitPathFinder
    {
        public CircuitPathFinder() { }

        /// <summary>
        /// 入力パターン選択,CircleDataのValueの初期化
        /// </summary>
        /// <param name="circles"></param>
        /// <param name="patternes"></param>
        /// <param name="selectPatternIndex"></param>
        private void Initialize(List<CircleData> circles, CirclePatternes circlePatternes, int selectPatternIndex)
        {
            //回路データの値の初期化
            foreach (var c in circles) { c.Value = false; }
            //外部入力値設定
            for (int i = 0; i < circlePatternes.Patternes[selectPatternIndex].Count; i++)
            {
                var value = circlePatternes.Patternes[selectPatternIndex][i] == 1 ? true : false;
                circles[i].Value = value;
            }
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
        /// CircleDataの値を決定し、処理済みのフラグを立てる。
        /// </summary>
        /// <param name="circles"></param>
        /// <param name="i"></param>
        private void FindPath(List<CircleData> circles, int i)
        {
            var type = circles[i].CircuitType;
            var inputs = SelectInputs(circles, i);
            circles[i].Value = GateFunctions.Execute(type, inputs);            
        }

        /// <summary>
        /// 論理シミュレータを実行して、入力パターンから出力を得る。
        /// </summary>
        /// <param name="circles">回路データリスト</param>
        /// <param name="patternes">入力パターン</param>
        /// <param name="selectPatternIndex">パターンの選択番号</param>
        /// <returns>論理回路の出力結果</returns>
        public List<CircleData> Simulation(List<CircleData> circles, CirclePatternes circlePatternes, int selectPatternIndex)
        {
            Initialize(circles, circlePatternes, selectPatternIndex);

            for (int i = 0; i < circles.Count; i++)
            {
                FindPath(circles, i);                
            }            
            //出力のみを取得する
            var result = new List<CircleData>();
            foreach (var c in circles)
            {
                if (c.CircuitType == CircuitType.PO) { result.Add(c); }
            }

            result.Sort((a, b) => a.Inputs[0] - b.Inputs[0]);
            return result;
        }
    }
}
